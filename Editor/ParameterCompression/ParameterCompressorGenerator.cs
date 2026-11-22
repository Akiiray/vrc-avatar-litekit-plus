using System;
using System.Collections.Generic;
using System.Linq;
using nadena.dev.ndmf;
using Akiiray.VRCAvatarLiteKitPlus.Editor.Localization;
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
        internal static void Apply(BuildContext context, VRCAvatarDescriptor descriptor, LiteKitParameterCompressor settings)
        {
            var scanned = ParameterScanner.Scan(descriptor);
            var plan = CompressionPlan.Create(scanned, settings.ExcludedParameters, settings.SyncChannelCount);
            var validation = CompressionValidation.Check(descriptor, plan, true);
            if (validation.Errors.Count > 0)
                throw new InvalidOperationException("[LiteKit Parameter Compressor]\n" + string.Join("\n", validation.Errors));

            if (!plan.IsUseful)
            {
                var key = plan.Targets.Count == 0 ? LiteKitTextKey.NoCompressionTargets : LiteKitTextKey.NoSavings;
                Debug.Log("[LiteKit Parameter Compressor] " + LiteKitLocalization.Get(LiteKitLocalization.CurrentLanguage, key));
                return;
            }

            var originalController = ParameterScanner.GetFxController(descriptor);
            var expressionParameters = UnityEngine.Object.Instantiate(descriptor.expressionParameters);
            expressionParameters.name = descriptor.expressionParameters.name + " (LiteKit Build Clone)";
            var controller = UnityEngine.Object.Instantiate(originalController);
            controller.name = originalController.name + " (LiteKit Build Clone)";

            RewriteExpressionParameters(expressionParameters, plan);
            var generatedAssets = GenerateAnimator(controller, plan);
            context.AssetSaver.SaveAsset(expressionParameters);
            context.AssetSaver.SaveAsset(controller);
            foreach (var generatedAsset in generatedAssets) context.AssetSaver.SaveAsset(generatedAsset);

            AssignFxController(descriptor, controller);
            descriptor.expressionParameters = expressionParameters;
            Debug.Log($"[LiteKit Parameter Compressor] {validation.OriginalBits} → {validation.FinalBits} bits (-{plan.SavedBits})");
        }

        private static void RewriteExpressionParameters(VRCExpressionParameters asset, CompressionPlan plan)
        {
            var targetNames = new HashSet<string>(plan.Targets.Select(p => p.Name));
            var parameters = new List<VRCExpressionParameters.Parameter>(asset.parameters);
            foreach (var parameter in parameters)
                if (targetNames.Contains(parameter.name)) parameter.networkSynced = false;
            foreach (var parameter in plan.GetSyncParameters())
                parameters.Add(NewParameter(parameter.Name, parameter.Type));
            asset.parameters = parameters.ToArray();
        }

        private static VRCExpressionParameters.Parameter NewParameter(string name, VRCExpressionParameters.ValueType type) =>
            new VRCExpressionParameters.Parameter { name = name, valueType = type, saved = false, defaultValue = 0f, networkSynced = true };

        private static List<UnityEngine.Object> GenerateAnimator(AnimatorController controller, CompressionPlan plan)
        {
            var generatedAssets = new List<UnityEngine.Object>();
            foreach (var parameter in plan.GetRequiredAnimatorParameters())
                EnsureParameter(controller, parameter.Name, parameter.Type);
            foreach (var parameter in plan.GetSyncParameters())
                controller.AddParameter(parameter.Name, CompressionValidation.ToAnimatorType(parameter.Type));

            var machine = new AnimatorStateMachine { name = CompressionConstants.LayerName + " StateMachine" };
            generatedAssets.Add(machine);
            var layer = new AnimatorControllerLayer { name = CompressionConstants.LayerName, defaultWeight = 1f, stateMachine = machine };
            controller.AddLayer(layer);
            var idle = machine.AddState("Idle", new Vector3(0, 0));
            var localIdle = machine.AddState("LocalIdle", new Vector3(250, -100));
            var remote = machine.AddState("Remote", new Vector3(250, 150));
            generatedAssets.Add(idle); generatedAssets.Add(localIdle); generatedAssets.Add(remote);
            machine.defaultState = idle;
            generatedAssets.Add(AddConditionTransition(idle, localIdle, AnimatorConditionMode.If, 0, "IsLocal"));
            generatedAssets.Add(AddConditionTransition(idle, remote, AnimatorConditionMode.IfNot, 0, "IsLocal"));

            var encoderStates = new List<AnimatorState>();
            for (var index = 0; index < plan.Batches.Count; index++)
            {
                var batch = plan.Batches[index];
                var encoder = machine.AddState("Batch_" + batch.Index, new Vector3(500 + index * 180, -100));
                encoder.motion = CreateTimingClip("LiteKit_PC_Batch_" + batch.Index);
                generatedAssets.Add(encoder); generatedAssets.Add(encoder.motion);
                generatedAssets.Add(ConfigureDriver(encoder, batch, true));
                encoderStates.Add(encoder);

                var decoder = machine.AddState("Set_B" + batch.Index, new Vector3(500 + index * 180, 150));
                generatedAssets.Add(decoder);
                generatedAssets.Add(ConfigureDriver(decoder, batch, false));
                generatedAssets.Add(AddConditionTransition(remote, decoder, AnimatorConditionMode.Equals, batch.Index, CompressionConstants.IndexParameter));
                generatedAssets.Add(AddConditionTransition(decoder, remote, AnimatorConditionMode.NotEqual, batch.Index, CompressionConstants.IndexParameter));
            }

            // Exit Timeには有効なMotionの長さが必要。Batch数によらず同じ保持時間を使う。
            localIdle.motion = CreateTimingClip("LiteKit_PC_LocalIdle");
            generatedAssets.Add(localIdle.motion);
            generatedAssets.Add(AddExitTimeTransition(localIdle, encoderStates[0]));
            if (encoderStates.Count == 1)
            {
                generatedAssets.Add(AddExitTimeTransition(encoderStates[0], localIdle));
            }
            else
            {
                for (var i = 0; i < encoderStates.Count; i++)
                    generatedAssets.Add(AddExitTimeTransition(encoderStates[i], encoderStates[(i + 1) % encoderStates.Count]));
            }
            return generatedAssets;
        }

        private static VRCAvatarParameterDriver ConfigureDriver(AnimatorState state, CompressionBatch batch, bool encoder)
        {
            var driver = state.AddStateMachineBehaviour<VRCAvatarParameterDriver>();
            driver.localOnly = encoder;
            driver.parameters = new List<VRC_AvatarParameterDriver.Parameter>();
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
            // ローカルDriver内ではChannelを先に更新する。ネットワーク上の同時到着は保証されない。
            if (encoder)
                driver.parameters.Add(new VRC_AvatarParameterDriver.Parameter
                    { name = CompressionConstants.IndexParameter, type = VRC_AvatarParameterDriver.ChangeType.Set, value = batch.Index });
            return driver;
        }

        private static AnimationClip CreateTimingClip(string name)
        {
            var clip = new AnimationClip { name = name, frameRate = 30f };
            // 存在しない予約パスの曲線で長さだけを確保し、Avatar の実オブジェクトを変更しない。
            clip.SetCurve(CompressionConstants.TimingPath, typeof(Transform), "m_LocalPosition.x",
                AnimationCurve.Constant(0f, CompressionConstants.BatchHoldSeconds, 0f));
            return clip;
        }

        private static void EnsureParameter(AnimatorController controller, string name, AnimatorControllerParameterType type)
        {
            var existing = controller.parameters.FirstOrDefault(p => p.name == name);
            if (existing == null) controller.AddParameter(name, type);
            else if (existing.type != type) throw new InvalidOperationException($"{name}: {existing.type} / {type}");
        }

        private static AnimatorStateTransition AddConditionTransition(AnimatorState from, AnimatorState to, AnimatorConditionMode mode, float threshold, string parameter)
        {
            var transition = from.AddTransition(to);
            transition.hasExitTime = false;
            transition.duration = 0f;
            transition.AddCondition(mode, threshold, parameter);
            return transition;
        }

        private static AnimatorStateTransition AddExitTimeTransition(AnimatorState from, AnimatorState to)
        {
            var transition = from.AddTransition(to);
            transition.hasExitTime = true;
            transition.exitTime = 1f;
            transition.hasFixedDuration = true;
            transition.duration = 0f;
            return transition;
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
