using UnityEngine;
using jp.unisakistudio.vrcavatarlitekitplus.Editor.Core;

namespace jp.unisakistudio.vrcavatarlitekitplus.Editor.Utilities
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
