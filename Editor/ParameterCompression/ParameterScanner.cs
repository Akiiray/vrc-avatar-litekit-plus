using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;
using VRC.SDK3.Avatars.Components;
using VRC.SDK3.Avatars.ScriptableObjects;

namespace Akiiray.VRCAvatarLiteKitPlus.Editor.ParameterCompression
{
    internal static class ParameterScanner
    {
        internal static List<ScannedParameter> Scan(VRCAvatarDescriptor descriptor)
        {
            var result = new List<ScannedParameter>();
            var names = new HashSet<string>();
            var expressionParameters = descriptor != null ? descriptor.expressionParameters : null;
            if (expressionParameters != null && expressionParameters.parameters != null)
            {
                foreach (var parameter in expressionParameters.parameters)
                {
                    if (parameter == null || string.IsNullOrEmpty(parameter.name) || !names.Add(parameter.name)) continue;
                    result.Add(new ScannedParameter
                    {
                        Name = parameter.name, Type = parameter.valueType, NetworkSynced = parameter.networkSynced,
                        Saved = parameter.saved, DefaultValue = parameter.defaultValue,
                        Source = "VRCExpressionParameters", SourceObject = expressionParameters
                    });
                }
            }

            var fx = GetFxController(descriptor);
            if (fx != null)
            {
                foreach (var parameter in fx.parameters)
                {
                    if (!names.Add(parameter.name)) continue;
                    if (!TryConvert(parameter.type, out var type)) continue;
                    result.Add(new ScannedParameter
                    {
                        Name = parameter.name, Type = type, NetworkSynced = false, Saved = false,
                        DefaultValue = parameter.defaultFloat, Source = "FX AnimatorController", SourceObject = fx
                    });
                }
            }
            return result;
        }

        internal static AnimatorController GetFxController(VRCAvatarDescriptor descriptor)
        {
            if (descriptor == null || descriptor.baseAnimationLayers == null) return null;
            foreach (var layer in descriptor.baseAnimationLayers)
                if (layer.type == VRCAvatarDescriptor.AnimLayerType.FX) return layer.animatorController as AnimatorController;
            return null;
        }

        private static bool TryConvert(AnimatorControllerParameterType input, out VRCExpressionParameters.ValueType output)
        {
            if (input == AnimatorControllerParameterType.Bool || input == AnimatorControllerParameterType.Trigger)
                output = VRCExpressionParameters.ValueType.Bool;
            else if (input == AnimatorControllerParameterType.Int) output = VRCExpressionParameters.ValueType.Int;
            else if (input == AnimatorControllerParameterType.Float) output = VRCExpressionParameters.ValueType.Float;
            else { output = default; return false; }
            return true;
        }
    }
}
