using UnityEngine;
using Akiiray.VRCAvatarLiteKitPlus.Editor.Core;

namespace Akiiray.VRCAvatarLiteKitPlus.Editor.Utilities
{
    public static class LiteKitLogger
    {
        public static void Info(string message)
        {
            Debug.Log($"[{LiteKitConstants.ToolName}] {message}");
        }

        public static void Warning(string message)
        {
            Debug.LogWarning($"[{LiteKitConstants.ToolName}] {message}");
        }
    }
}
