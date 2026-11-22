using System;
using System.Collections.Generic;
using Akiiray.VRCAvatarLiteKitPlus.Editor.Localization;
using VRC.SDK3.Avatars.ScriptableObjects;

namespace Akiiray.VRCAvatarLiteKitPlus.Editor.ParameterCompression
{
    internal static class ParameterExclusionRules
    {
        internal static bool IsFaceTracking(string name) =>
            !string.IsNullOrEmpty(name) && (name.StartsWith("v2/", StringComparison.Ordinal) ||
            name.IndexOf("/v2/", StringComparison.Ordinal) >= 0);

        internal static bool IsMenuBuilder(string name) => name == "MB_Sync_Index" || name == "MB_Sync_Value";

        internal static bool IsVrcFurySync(string name) => !string.IsNullOrEmpty(name) &&
            (name.StartsWith("SyncDataNum", StringComparison.Ordinal) ||
             name.StartsWith("SyncIndex", StringComparison.Ordinal) || name == "SyncPointer");

        internal static LiteKitTextKey? GetExclusion(ScannedParameter parameter, ICollection<string> userExcluded)
        {
            var name = parameter.Name;
            if (IsReservedName(name)) return LiteKitTextKey.CompressionExcludedReserved;
            if (BuiltInParameterRegistry.Contains(name)) return LiteKitTextKey.CompressionExcludedBuiltIn;
            if (parameter.ContactDriven) return LiteKitTextKey.CompressionExcludedContact;
            if (IsFaceTracking(name)) return LiteKitTextKey.CompressionExcludedFaceTracking;
            if (IsMenuBuilder(name) || IsVrcFurySync(name) ||
                name.StartsWith("PC_Sync_", StringComparison.Ordinal) ||
                name.StartsWith("LightLimitChanger/System/", StringComparison.Ordinal))
                return LiteKitTextKey.CompressionExcludedTool;
            if (parameter.IsTrigger || (parameter.Type != VRCExpressionParameters.ValueType.Float &&
                parameter.Type != VRCExpressionParameters.ValueType.Int))
                return LiteKitTextKey.CompressionExcludedType;
            if (!parameter.NetworkSynced) return LiteKitTextKey.CompressionExcludedLocal;
            if (parameter.BitCost != 8) return LiteKitTextKey.CompressionExcludedBitWidth;
            if (userExcluded != null && userExcluded.Contains(name)) return LiteKitTextKey.CompressionExcludedManual;
            return null;
        }

        internal static bool IsReservedName(string name) =>
            !string.IsNullOrEmpty(name) && name.StartsWith(CompressionConstants.ReservedPrefix, StringComparison.Ordinal);
    }
}
