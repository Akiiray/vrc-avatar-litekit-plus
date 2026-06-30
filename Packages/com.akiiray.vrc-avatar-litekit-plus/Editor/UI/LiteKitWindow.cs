using UnityEditor;
using UnityEngine;
using com.akiiray.vrcavatarlitekitplus.Editor.Core;
using com.akiiray.vrcavatarlitekitplus.Editor.Utilities;

namespace com.akiiray.vrcavatarlitekitplus.Editor.UI
{
    public sealed class LiteKitWindow : EditorWindow
    {
        [MenuItem(LiteKitConstants.MenuRoot + "/Open LiteKit Window")]
        public static void Open()
        {
            GetWindow<LiteKitWindow>(LiteKitConstants.ToolName);
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField(LiteKitConstants.ToolName, EditorStyles.boldLabel);
            EditorGUILayout.LabelField($"Version {LiteKitConstants.Version} - Initial development preview");
            EditorGUILayout.HelpBox("This package currently provides repository structure and safe extension points. Optimization algorithms are not implemented yet.", MessageType.Info);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Planned Categories", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("• Avatar Performance Analyzer");
            EditorGUILayout.LabelField("• Performance Target Planner");
            EditorGUILayout.LabelField("• Texture / UV Usage Analysis");
            EditorGUILayout.LabelField("• Material Duplicate Finder");
            EditorGUILayout.LabelField("• Mobile Texture Profiles");
            EditorGUILayout.LabelField("• NDMF build-time optimization placeholders");

            EditorGUILayout.Space();
            if (GUILayout.Button("Analyze Avatar"))
            {
                LiteKitLogger.Info("Analyze Avatar is not implemented yet. Future versions will run a dry-run analyzer and Before/After report.");
            }
        }
    }
}
