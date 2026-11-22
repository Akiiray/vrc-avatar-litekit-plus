using System;
using System.Collections.Generic;
using System.Linq;
using Akiiray.VRCAvatarLiteKitPlus.Editor.Localization;
using UnityEditor.Animations;
using UnityEngine;
using VRC.SDK3.Avatars.Components;
using VRC.SDK3.Avatars.ScriptableObjects;

namespace Akiiray.VRCAvatarLiteKitPlus.Editor.ParameterCompression
{
    internal sealed class CompressionValidation
    {
        internal readonly List<string> Errors = new List<string>();
        internal int OriginalBits;
        internal int FinalBits;

        internal static CompressionValidation Check(VRCAvatarDescriptor descriptor, CompressionPlan plan, bool enabled)
        {
            var result = new CompressionValidation();
            var asset = descriptor != null ? descriptor.expressionParameters : null;
            var parameters = asset != null ? asset.parameters : null;
            var invalidRows = Array.Exists(parameters ?? Array.Empty<VRCExpressionParameters.Parameter>(), p => p == null);
            result.OriginalBits = parameters != null && !invalidRows ? asset.CalcTotalCost() : 0;
            result.FinalBits = result.OriginalBits;
            if (!enabled) return result;

            if (descriptor == null)
            {
                result.AddError(LiteKitTextKey.DescriptorRequired);
                return result;
            }
            if (asset == null)
            {
                result.AddError(LiteKitTextKey.CompressionExpressionRequired);
                return result;
            }
            if (invalidRows)
            {
                result.AddError(LiteKitTextKey.CompressionInvalidExpression);
                return result;
            }
            if (plan.IsUseful) result.FinalBits -= plan.SavedBits;

            var expected = plan.GetRequiredAnimatorParameters().ToDictionary(p => p.Name, p => p.Type);
            var sources = new HashSet<string>();
            foreach (var parameter in parameters ?? Array.Empty<VRCExpressionParameters.Parameter>())
            {
                if (parameter == null || string.IsNullOrEmpty(parameter.name)) continue;
                result.CheckReserved(parameter.name, asset.name);
                if (!plan.IsUseful || !expected.TryGetValue(parameter.name, out var type)) continue;
                if (!sources.Add(parameter.name))
                    result.AddError(LiteKitTextKey.CompressionDuplicateParameter, parameter.name, asset.name);
                result.CheckType(parameter.name, ToAnimatorType(parameter.valueType), type, asset.name);
            }

            var controllers = new HashSet<AnimatorController>();
            foreach (var slot in (descriptor.baseAnimationLayers ?? Array.Empty<VRCAvatarDescriptor.CustomAnimLayer>())
                .Concat(descriptor.specialAnimationLayers ?? Array.Empty<VRCAvatarDescriptor.CustomAnimLayer>()))
            {
                var runtimeController = slot.animatorController;
                while (runtimeController is AnimatorOverrideController overrides)
                    runtimeController = overrides.runtimeAnimatorController;
                if (runtimeController is AnimatorController controller && controllers.Add(controller))
                    result.CheckController(controller, expected, plan.IsUseful);
            }

            foreach (var name in ParameterScanner.GetContactNames(descriptor))
                result.CheckReserved(name, "VRCContactReceiver");
            result.CheckMenu(descriptor.expressionsMenu, new HashSet<VRCExpressionsMenu>());

            if (plan.Batches.Count > CompressionConstants.MaximumBatchIndex)
                result.AddError(LiteKitTextKey.BatchLimit);
            if (!plan.IsUseful) return result;
            if (ParameterScanner.GetFxController(descriptor) == null)
                result.AddError(LiteKitTextKey.CompressionFxRequired);
            if (descriptor.transform.Find(CompressionConstants.TimingPath) != null)
                result.AddError(LiteKitTextKey.CompressionTimingConflict, CompressionConstants.TimingPath);
            if (result.FinalBits > CompressionConstants.ParameterBudgetBits)
                result.AddError(LiteKitTextKey.BudgetExceeded);
            return result;
        }

        internal static AnimatorControllerParameterType ToAnimatorType(VRCExpressionParameters.ValueType type)
        {
            switch (type)
            {
                case VRCExpressionParameters.ValueType.Float: return AnimatorControllerParameterType.Float;
                case VRCExpressionParameters.ValueType.Int: return AnimatorControllerParameterType.Int;
                default: return AnimatorControllerParameterType.Bool;
            }
        }

        private void CheckController(AnimatorController controller, Dictionary<string, AnimatorControllerParameterType> expected, bool willGenerate)
        {
            if (controller.layers.Any(layer => layer.name == CompressionConstants.LayerName))
                AddError(LiteKitTextKey.CompressionLayerConflict, CompressionConstants.LayerName, controller.name);

            var names = new HashSet<string>();
            foreach (var parameter in controller.parameters)
            {
                CheckReserved(parameter.name, controller.name);
                if (!willGenerate || string.IsNullOrEmpty(parameter.name) ||
                    !expected.TryGetValue(parameter.name, out var type)) continue;
                if (!names.Add(parameter.name))
                    AddError(LiteKitTextKey.CompressionDuplicateParameter, parameter.name, controller.name);
                CheckType(parameter.name, parameter.type, type, controller.name);
            }
        }

        private void CheckMenu(VRCExpressionsMenu menu, HashSet<VRCExpressionsMenu> visited)
        {
            if (menu == null || !visited.Add(menu) || menu.controls == null) return;
            foreach (var control in menu.controls)
            {
                if (control == null) continue;
                CheckReserved(control.parameter?.name, menu.name);
                if (control.subParameters != null)
                    foreach (var parameter in control.subParameters) CheckReserved(parameter?.name, menu.name);
                CheckMenu(control.subMenu, visited);
            }
        }

        private void CheckReserved(string name, string source)
        {
            if (ParameterExclusionRules.IsReservedName(name))
                AddError(LiteKitTextKey.CompressionReservedConflict, name, source);
        }

        private void CheckType(string name, AnimatorControllerParameterType actual, AnimatorControllerParameterType expected, string source)
        {
            if (actual != expected)
                AddError(LiteKitTextKey.CompressionTypeConflict, name, source, actual, expected);
        }

        private void AddError(LiteKitTextKey key, params object[] args)
        {
            var message = string.Format(LiteKitLocalization.Get(LiteKitLocalization.CurrentLanguage, key), args);
            if (!Errors.Contains(message)) Errors.Add(message);
        }
    }
}
