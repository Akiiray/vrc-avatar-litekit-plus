using System.Collections.Generic;

namespace Akiiray.VRCAvatarLiteKitPlus.Editor.ParameterCompression
{
    internal static class BuiltInParameterRegistry
    {
        private static readonly HashSet<string> Names = new HashSet<string>
        {
            "IsLocal", "PreviewMode", "Viseme", "Voice", "GestureLeft", "GestureRight",
            "GestureLeftWeight", "GestureRightWeight", "AngularY", "VelocityX", "VelocityY",
            "VelocityZ", "VelocityMagnitude", "Upright", "Grounded", "Seated", "AFK",
            "Expression1", "Expression2", "Expression3", "TrackingType", "VRMode", "MuteSelf",
            "InStation", "Earmuffs", "IsOnFriendsList", "IsAnimatorEnabled", "IsImmobilized", "AvatarVersion", "ScaleModified",
            "ScaleFactor", "ScaleFactorInverse", "EyeHeightAsMeters", "EyeHeightAsPercent"
        };

        internal static bool Contains(string name) => !string.IsNullOrEmpty(name) && Names.Contains(name);
    }
}
