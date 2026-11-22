using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;
using VRC.SDK3.Avatars.Components;
using VRC.SDK3.Avatars.ScriptableObjects;
using VRC.SDK3.Dynamics.Contact.Components;

namespace Akiiray.VRCAvatarLiteKitPlus.Editor.ParameterCompression
{
    internal static class ParameterScanner
    {
        internal static List<ScannedParameter> Scan(VRCAvatarDescriptor descriptor)
        {
            var result = new List<ScannedParameter>();
            var names = new HashSet<string>();
            var contactNames = GetContactNames(descriptor);
            var expressionParameters = descriptor != null ? descriptor.expressionParameters : null;
            if (expressionParameters != null && expressionParameters.parameters != null)
                ScanExpressionParameters(expressionParameters, contactNames, names, result);

            var fx = GetFxController(descriptor);
            if (fx != null)
            {
                foreach (var parameter in fx.parameters)
                {
                    if (string.IsNullOrEmpty(parameter.name) || !names.Add(parameter.name)) continue;
                    if (!TryConvert(parameter.type, out var type)) continue;
                    result.Add(new ScannedParameter
                    {
                        Name = parameter.name, Type = type, NetworkSynced = false, Saved = false,
                        BitCost = type == VRCExpressionParameters.ValueType.Bool ? 1 : 8,
                        IsTrigger = parameter.type == AnimatorControllerParameterType.Trigger,
                        ContactDriven = contactNames.Contains(parameter.name),
                        DefaultValue = parameter.defaultFloat, Source = "FX AnimatorController", SourceObject = fx
                    });
                }
            }
            return result;
        }

        private static void ScanExpressionParameters(VRCExpressionParameters expressionParameters, HashSet<string> contactNames,
            HashSet<string> names, List<ScannedParameter> result)
        {
            // Use the SDK's public cost calculation on a one-parameter scratch asset. SDKs with
            // configurable bit widths can differ from the legacy 8-bit Float / Int defaults.
            var costSample = ScriptableObject.CreateInstance<VRCExpressionParameters>();
            var sampleParameters = new VRCExpressionParameters.Parameter[1];
            try
            {
                foreach (var parameter in expressionParameters.parameters)
                {
                    if (parameter == null || string.IsNullOrEmpty(parameter.name) || !names.Add(parameter.name)) continue;
                    sampleParameters[0] = parameter;
                    costSample.parameters = sampleParameters;
                    result.Add(new ScannedParameter
                    {
                        Name = parameter.name, Type = parameter.valueType, NetworkSynced = parameter.networkSynced,
                        Saved = parameter.saved, DefaultValue = parameter.defaultValue, BitCost = costSample.CalcTotalCost(),
                        ContactDriven = contactNames.Contains(parameter.name),
                        Source = "VRCExpressionParameters", SourceObject = expressionParameters
                    });
                }
            }
            finally
            {
                Object.DestroyImmediate(costSample);
            }
        }

        internal static HashSet<string> GetContactNames(VRCAvatarDescriptor descriptor)
        {
            var names = new HashSet<string>();
            if (descriptor == null) return names;
            foreach (var receiver in descriptor.GetComponentsInChildren<VRCContactReceiver>(true))
                if (!string.IsNullOrEmpty(receiver.parameter)) names.Add(receiver.parameter);
            return names;
        }

        internal static AnimatorController GetFxController(VRCAvatarDescriptor descriptor)
        {
            if (descriptor == null || descriptor.baseAnimationLayers == null) return null;
            foreach (var layer in descriptor.baseAnimationLayers)
                if (layer.type == VRCAvatarDescriptor.AnimLayerType.FX)
                    return layer.isDefault ? null : layer.animatorController as AnimatorController;
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
