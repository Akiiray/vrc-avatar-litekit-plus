using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using VRC.SDK3.Avatars.Components;
using VRC.SDK3.Avatars.ScriptableObjects;
using VRC.SDKBase;

namespace Akiiray.VRCAvatarLiteKitPlus.Editor.ParameterCompression
{
    internal static class ParameterCompressorGenerator
    {
        internal static bool Apply(VRCAvatarDescriptor descriptor, LiteKitParameterCompressor settings, out string message)
        {
            message = null;
            if (descriptor == null) { message = "VRCAvatarDescriptor が見つかりません。"; return false; }
            if (settings == null || !settings.EnableCompression) { message = "Parameter Compressor は無効です。"; return false; }
            if (descriptor.expressionParameters == null) { message = "Expression Parameters が設定されていません。"; return false; }

            // NDMF のビルド用 Avatar 上でも、参照先が Project Asset の可能性を残さないため明示的に複製する。
            var expressionParameters = UnityEngine.Object.Instantiate(descriptor.expressionParameters);
            expressionParameters.name = descriptor.expressionParameters.name + " (LiteKit Build Clone)";
            descriptor.expressionParameters = expressionParameters;

            var scanned = ParameterScanner.Scan(descriptor);
            var plan = CompressionPlan.Create(scanned, settings.ExcludedParameters, settings.SyncChannelCount);
            if (plan.Targets.Count == 0) { message = "圧縮対象の同期 Float / Int Parameter がありません。"; return false; }
            if (plan.SavedBits <= 0) { message = "圧縮による bit 削減効果がないため処理をスキップしました。"; return false; }
            if (plan.Batches.Count > CompressionConstants.MaximumBatchIndex)
            {
                message = "Parameter Compressor で扱える Batch 数の上限を超えています。同期チャンネル数を増やすか、圧縮対象を減らしてください。";
                return false;
            }

            var originalController = ParameterScanner.GetFxController(descriptor);
            if (originalController == null) { message = "書き込み可能な FX AnimatorController を取得できません。"; return false; }
            var controller = UnityEngine.Object.Instantiate(originalController);
            controller.name = originalController.name + " (LiteKit Build Clone)";
            AssignFxController(descriptor, controller);

            RewriteExpressionParameters(expressionParameters, plan);
            GenerateAnimator(controller, plan);
            var finalBits = expressionParameters.parameters.Where(p => p.networkSynced)
                .Sum(p => p.valueType == VRCExpressionParameters.ValueType.Bool ? 1 : 8);
            message = $"Parameter Compressor: {plan.OriginalBits}bit から {plan.CompressedBits}bit へ圧縮しました（{plan.SavedBits}bit 削減、最終同期使用量 {finalBits}bit）。";
            if (finalBits > CompressionConstants.ParameterBudgetBits)
                message += " 圧縮後もParameter予算256bitを超過しています。";
            return true;
        }

        private static void RewriteExpressionParameters(VRCExpressionParameters asset, CompressionPlan plan)
        {
            var targetNames = new HashSet<string>(plan.Targets.Select(p => p.Name));
            var parameters = new List<VRCExpressionParameters.Parameter>();
            foreach (var parameter in asset.parameters ?? Array.Empty<VRCExpressionParameters.Parameter>())
            {
                if (parameter == null || parameter.name.StartsWith(CompressionConstants.SyncPrefix, StringComparison.Ordinal)) continue;
                if (targetNames.Contains(parameter.name)) parameter.networkSynced = false;
                parameters.Add(parameter);
            }
            parameters.Add(NewParameter(CompressionConstants.IndexParameter, VRCExpressionParameters.ValueType.Int));
            for (var i = 0; i < Math.Min(plan.EffectiveChannelCount, plan.FloatCount); i++)
                parameters.Add(NewParameter(CompressionConstants.FloatChannelPrefix + i, VRCExpressionParameters.ValueType.Float));
            for (var i = 0; i < Math.Min(plan.EffectiveChannelCount, plan.IntCount); i++)
                parameters.Add(NewParameter(CompressionConstants.IntChannelPrefix + i, VRCExpressionParameters.ValueType.Int));
            asset.parameters = parameters.ToArray();
        }

        private static VRCExpressionParameters.Parameter NewParameter(string name, VRCExpressionParameters.ValueType type) =>
            new VRCExpressionParameters.Parameter { name = name, valueType = type, saved = false, defaultValue = 0f, networkSynced = true };

        private static void GenerateAnimator(AnimatorController controller, CompressionPlan plan)
        {
            controller.layers = controller.layers.Where(l => l.name != CompressionConstants.LayerName).ToArray();
            EnsureParameter(controller, "IsLocal", AnimatorControllerParameterType.Bool);
            EnsureParameter(controller, CompressionConstants.IndexParameter, AnimatorControllerParameterType.Int);
            foreach (var p in plan.Targets) EnsureParameter(controller, p.Name,
                p.Type == VRCExpressionParameters.ValueType.Float ? AnimatorControllerParameterType.Float : AnimatorControllerParameterType.Int);
            for (var i = 0; i < Math.Min(plan.EffectiveChannelCount, plan.FloatCount); i++)
                EnsureParameter(controller, CompressionConstants.FloatChannelPrefix + i, AnimatorControllerParameterType.Float);
            for (var i = 0; i < Math.Min(plan.EffectiveChannelCount, plan.IntCount); i++)
                EnsureParameter(controller, CompressionConstants.IntChannelPrefix + i, AnimatorControllerParameterType.Int);

            var machine = new AnimatorStateMachine { name = CompressionConstants.LayerName + " StateMachine" };
            var layer = new AnimatorControllerLayer { name = CompressionConstants.LayerName, defaultWeight = 1f, stateMachine = machine };
            controller.AddLayer(layer);
            var idle = machine.AddState("Idle", new Vector3(0, 0));
            var localIdle = machine.AddState("LocalIdle", new Vector3(250, -100));
            var remote = machine.AddState("Remote", new Vector3(250, 150));
            machine.defaultState = idle;
            AddConditionTransition(idle, localIdle, AnimatorConditionMode.If, 0, "IsLocal");
            AddConditionTransition(idle, remote, AnimatorConditionMode.IfNot, 0, "IsLocal");

            var encoderStates = new List<AnimatorState>();
            for (var index = 0; index < plan.Batches.Count; index++)
            {
                var batch = plan.Batches[index];
                var encoder = machine.AddState("Batch_" + batch.Index, new Vector3(500 + index * 180, -100));
                encoder.motion = CreateTimingClip("LiteKit_PC_Batch_" + batch.Index);
                ConfigureDriver(encoder, batch, true);
                encoderStates.Add(encoder);

                var decoder = machine.AddState("Set_B" + batch.Index, new Vector3(500 + index * 180, 150));
                ConfigureDriver(decoder, batch, false);
                AddConditionTransition(remote, decoder, AnimatorConditionMode.Equals, batch.Index, CompressionConstants.IndexParameter);
                AddConditionTransition(decoder, remote, AnimatorConditionMode.NotEqual, batch.Index, CompressionConstants.IndexParameter);
            }

            AddExitTimeTransition(localIdle, encoderStates[0]);
            if (encoderStates.Count == 1)
            {
                localIdle.motion = CreateTimingClip("LiteKit_PC_LocalIdle");
                AddExitTimeTransition(encoderStates[0], localIdle);
            }
            else
            {
                for (var i = 0; i < encoderStates.Count; i++)
                    AddExitTimeTransition(encoderStates[i], encoderStates[(i + 1) % encoderStates.Count]);
            }
        }

        private static void ConfigureDriver(AnimatorState state, CompressionBatch batch, bool encoder)
        {
            var driver = state.AddStateMachineBehaviour<VRCAvatarParameterDriver>();
            driver.localOnly = encoder;
            driver.parameters = new List<VRC_AvatarParameterDriver.Parameter>();
            if (encoder)
                driver.parameters.Add(new VRC_AvatarParameterDriver.Parameter
                    { name = CompressionConstants.IndexParameter, type = VRC_AvatarParameterDriver.ChangeType.Set, value = batch.Index });
            for (var channel = 0; channel < batch.Parameters.Count; channel++)
            {
                var p = batch.Parameters[channel];
                var channelName = p.Type == VRCExpressionParameters.ValueType.Float
                    ? CompressionConstants.FloatChannelPrefix + channel : CompressionConstants.IntChannelPrefix + channel;
                driver.parameters.Add(new VRC_AvatarParameterDriver.Parameter
                {
                    name = encoder ? channelName : p.Name,
                    source = encoder ? p.Name : channelName,
                    type = VRC_AvatarParameterDriver.ChangeType.Copy
                });
            }
        }

        private static AnimationClip CreateTimingClip(string name)
        {
            var clip = new AnimationClip { name = name, frameRate = 30f };
            // 存在しない予約パスの曲線で長さだけを確保し、Avatar の実オブジェクトを変更しない。
            clip.SetCurve("__LiteKit_PC_Timing__", typeof(Transform), "m_LocalPosition.x",
                AnimationCurve.Constant(0f, CompressionConstants.BatchHoldSeconds, 0f));
            return clip;
        }

        private static void EnsureParameter(AnimatorController controller, string name, AnimatorControllerParameterType type)
        {
            var existing = controller.parameters.FirstOrDefault(p => p.name == name);
            if (existing != null) return;
            controller.AddParameter(name, type);
        }

        private static void AddConditionTransition(AnimatorState from, AnimatorState to, AnimatorConditionMode mode, float threshold, string parameter)
        {
            var transition = from.AddTransition(to);
            transition.hasExitTime = false;
            transition.duration = 0f;
            transition.AddCondition(mode, threshold, parameter);
        }

        private static void AddExitTimeTransition(AnimatorState from, AnimatorState to)
        {
            var transition = from.AddTransition(to);
            transition.hasExitTime = true;
            transition.exitTime = 1f;
            transition.hasFixedDuration = true;
            transition.duration = 0f;
        }

        private static void AssignFxController(VRCAvatarDescriptor descriptor, AnimatorController controller)
        {
            var layers = descriptor.baseAnimationLayers;
            for (var i = 0; i < layers.Length; i++)
            {
                if (layers[i].type != VRCAvatarDescriptor.AnimLayerType.FX) continue;
                layers[i].animatorController = controller;
                layers[i].isDefault = false;
                descriptor.baseAnimationLayers = layers;
                return;
            }
        }
    }
}
