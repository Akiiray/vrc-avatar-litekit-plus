using System;
using System.Collections.Generic;
using UnityEngine;
using VRC.SDK3.Avatars.ScriptableObjects;

namespace Akiiray.VRCAvatarLiteKitPlus.Editor.ParameterCompression
{
    internal sealed class CompressionBatch
    {
        internal int Index;
        internal readonly List<ScannedParameter> Parameters = new List<ScannedParameter>();
    }

    internal sealed class CompressionPlan
    {
        internal readonly List<ScannedParameter> Targets = new List<ScannedParameter>();
        internal readonly List<CompressionBatch> Batches = new List<CompressionBatch>();
        internal int FloatCount;
        internal int IntCount;
        internal int EffectiveChannelCount;
        internal int MaxUsefulChannelCount;
        internal int FloatChannelCount => Math.Min(EffectiveChannelCount, FloatCount);
        internal int IntChannelCount => Math.Min(EffectiveChannelCount, IntCount);
        internal int OriginalBits => Targets.Count * 8;
        internal int CompressedBits => Targets.Count == 0 ? 0 : 8 + (FloatChannelCount + IntChannelCount) * 8;
        internal int SavedBits => OriginalBits - CompressedBits;
        internal float CycleSeconds => Batches.Count == 1 ? CompressionConstants.BatchHoldSeconds * 2f : Batches.Count * CompressionConstants.BatchHoldSeconds;
        internal bool IsUseful => Targets.Count > 0 && SavedBits > 0 && Batches.Count <= CompressionConstants.MaximumBatchIndex;

        internal static CompressionPlan Create(IList<ScannedParameter> scanned, ICollection<string> excluded, int requestedChannels)
        {
            var plan = new CompressionPlan();
            foreach (var p in scanned)
            {
                if (ParameterExclusionRules.GetExclusion(p, excluded) != null) continue;
                plan.Targets.Add(p);
                if (p.Type == VRCExpressionParameters.ValueType.Float) plan.FloatCount++;
                else if (p.Type == VRCExpressionParameters.ValueType.Int) plan.IntCount++;
            }
            plan.MaxUsefulChannelCount = FindMaxUsefulK(plan.FloatCount, plan.IntCount);
            plan.EffectiveChannelCount = plan.MaxUsefulChannelCount == 0 ? 1 : Math.Max(1, Math.Min(requestedChannels, plan.MaxUsefulChannelCount));
            AddBatches(plan, VRCExpressionParameters.ValueType.Float);
            AddBatches(plan, VRCExpressionParameters.ValueType.Int);
            return plan;
        }

        internal IEnumerable<(string Name, AnimatorControllerParameterType Type)> GetRequiredAnimatorParameters()
        {
            yield return ("IsLocal", AnimatorControllerParameterType.Bool);
            foreach (var parameter in Targets)
                yield return (parameter.Name, CompressionValidation.ToAnimatorType(parameter.Type));
        }

        internal IEnumerable<(string Name, VRCExpressionParameters.ValueType Type)> GetSyncParameters()
        {
            yield return (CompressionConstants.IndexParameter, VRCExpressionParameters.ValueType.Int);
            for (var i = 0; i < FloatChannelCount; i++)
                yield return (CompressionConstants.FloatChannelPrefix + i, VRCExpressionParameters.ValueType.Float);
            for (var i = 0; i < IntChannelCount; i++)
                yield return (CompressionConstants.IntChannelPrefix + i, VRCExpressionParameters.ValueType.Int);
        }

        internal static int FindMaxUsefulK(int floatCount, int intCount)
        {
            var original = (floatCount + intCount) * 8;
            var max = Math.Max(floatCount, intCount);
            var useful = 0;
            for (var k = 1; k <= Math.Max(1, max); k++)
                if (original > 8 + Math.Min(k, floatCount) * 8 + Math.Min(k, intCount) * 8) useful = k;
            return useful;
        }

        private static void AddBatches(CompressionPlan plan, VRCExpressionParameters.ValueType type)
        {
            CompressionBatch batch = null;
            var inBatch = 0;
            foreach (var p in plan.Targets)
            {
                if (p.Type != type) continue;
                if (batch == null || inBatch == plan.EffectiveChannelCount)
                {
                    batch = new CompressionBatch { Index = plan.Batches.Count + 1 };
                    plan.Batches.Add(batch);
                    inBatch = 0;
                }
                batch.Parameters.Add(p);
                inBatch++;
            }
        }
    }
}
