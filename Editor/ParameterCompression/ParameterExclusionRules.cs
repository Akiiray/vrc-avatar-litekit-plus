using System;
using System.Collections.Generic;

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

        internal static bool IsAutomaticallyExcluded(string name, ICollection<string> userExcluded)
        {
            return string.IsNullOrEmpty(name) || BuiltInParameterRegistry.Contains(name) ||
                   name.StartsWith(CompressionConstants.SyncPrefix, StringComparison.Ordinal) ||
                   name.StartsWith("LightLimitChanger/System/", StringComparison.Ordinal) ||
                   IsFaceTracking(name) || IsVrcFurySync(name) ||
                   (userExcluded != null && userExcluded.Contains(name));
        }
    }
}
