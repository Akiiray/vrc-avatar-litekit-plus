using System;
using UnityEditor;
using UnityEngine;
using Akiiray.VRCAvatarLiteKitPlus.Editor.Analyzer;
using Akiiray.VRCAvatarLiteKitPlus.Editor.Core;
using Akiiray.VRCAvatarLiteKitPlus.Editor.Planner;
using Akiiray.VRCAvatarLiteKitPlus.Editor.Utilities;

namespace Akiiray.VRCAvatarLiteKitPlus.Editor.UI
{
    public sealed class LiteKitWindow : EditorWindow
    {
        private static readonly string[] Tabs = { "Overview", "Performance", "Renderers", "Materials", "Textures", "Mobile Warnings", "Raw Report" };
        private static readonly string[] TargetRankLabels = { "Excellent or better", "Good or better", "Medium or better", "Poor or better" };
        private static readonly PerformanceRank[] TargetRankValues = { PerformanceRank.Excellent, PerformanceRank.Good, PerformanceRank.Medium, PerformanceRank.Poor };

        private GameObject avatarRoot;
        private LiteKitPlatform targetPlatform = LiteKitPlatform.PC;
        private PerformanceRank targetRank = PerformanceRank.Poor;
        private LiteKitReport report;
        private Vector2 scroll;
        private int selectedTab;
        private string windowLog = string.Empty;

        [MenuItem(LiteKitConstants.MenuRoot + "/Open LiteKit Window")]
        public static void Open()
        {
            GetWindow<LiteKitWindow>(LiteKitConstants.ToolName);
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField(LiteKitConstants.ToolName, EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("Dry Run only. LiteKit v0.1 estimates rank from public VRChat Performance Rank thresholds and does not run SDK Builder validation. Results may differ from the VRChat SDK.", MessageType.Info);

            DrawControls();
            EditorGUILayout.Space();
            DrawSummary();
            EditorGUILayout.Space();

            selectedTab = GUILayout.Toolbar(selectedTab, Tabs);
            scroll = EditorGUILayout.BeginScrollView(scroll);
            DrawSelectedTab();
            EditorGUILayout.EndScrollView();
        }

        private void DrawControls()
        {
            avatarRoot = (GameObject)EditorGUILayout.ObjectField("Avatar Root", avatarRoot, typeof(GameObject), true);
            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Use Selection.activeGameObject"))
                {
                    avatarRoot = Selection.activeGameObject;
                    if (avatarRoot == null)
                    {
                        LiteKitLogger.Warning("Avatar Root is not selected. Select an avatar root in the Hierarchy.");
                        windowLog = "Avatar Root is not selected.";
                    }
                }

                if (GUILayout.Button("Analyze Avatar"))
                {
                    AnalyzeAvatar();
                }
            }

            targetPlatform = (LiteKitPlatform)EditorGUILayout.EnumPopup("Target Platform", targetPlatform);
            var targetRankIndex = Mathf.Clamp(Array.IndexOf(TargetRankValues, targetRank), 0, TargetRankValues.Length - 1);
            targetRankIndex = EditorGUILayout.Popup("Target Rank", targetRankIndex, TargetRankLabels);
            targetRank = TargetRankValues[targetRankIndex];
        }

        private void AnalyzeAvatar()
        {
            if (avatarRoot == null)
            {
                LiteKitLogger.Warning("Avatar Root is not selected. Analysis canceled.");
                windowLog = "Avatar Root is not selected. Assign an Avatar Root or use the active selection button.";
                report = null;
                return;
            }

            LiteKitLogger.Info($"Analyze started: {avatarRoot.name}");
            var context = AvatarScanner.Scan(avatarRoot);
            if (context == null || !context.IsValid)
            {
                windowLog = "Avatar scan failed.";
                report = null;
                return;
            }

            var stats = new PerformanceStatsAnalyzer().Analyze(context);
            var evaluation = new PerformanceRankEvaluator().Evaluate(stats, targetPlatform, targetRank);
            var renderers = new RendererUsageAnalyzer().Analyze(context);
            var materials = new MaterialUsageAnalyzer().Analyze(context);
            var textures = new TextureUsageAnalyzer().Analyze(context);
            var mobileWarnings = new MobileCompatibilityAnalyzer().Analyze(context, materials);

            report = new LiteKitReport
            {
                GeneratedTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                AvatarName = avatarRoot.name,
                TargetPlatform = targetPlatform,
                TargetRank = targetRank,
                EstimatedRank = evaluation.EstimatedRank,
                Stats = stats,
                Evaluation = evaluation,
                Renderers = renderers,
                Materials = materials,
                Textures = textures,
                MobileWarnings = mobileWarnings,
                ScannerWarnings = new System.Collections.Generic.List<string>(context.Warnings),
                Notes = "This is a non-destructive Dry Run report. No Material, Texture, Mesh, Prefab, or Avatar asset was modified."
            };

            windowLog = $"Analyze completed. Estimated Rank: {evaluation.EstimatedRank}. Mobile warnings: {mobileWarnings.Count}.";
            LiteKitLogger.Info($"Analyze completed: {avatarRoot.name}");
            LiteKitLogger.Info($"Estimated Rank: {evaluation.EstimatedRank}");
            LiteKitLogger.Info($"Mobile warning count: {mobileWarnings.Count}");
        }

        private void DrawSummary()
        {
            EditorGUILayout.LabelField("Target Avatar", avatarRoot != null ? avatarRoot.name : "<none>");
            if (!string.IsNullOrEmpty(windowLog))
            {
                EditorGUILayout.HelpBox(windowLog, report == null ? MessageType.Warning : MessageType.Info);
            }

            if (report == null)
            {
                return;
            }

            EditorGUILayout.LabelField("Estimated Rank", report.EstimatedRank.ToString());
            EditorGUILayout.LabelField("Target Rank", report.TargetRank.ToString());
            EditorGUILayout.LabelField("Blocking Metrics", report.Evaluation.TargetPlan.BlockingMetrics.Count.ToString());
            EditorGUILayout.LabelField("Renderer count", report.Renderers.Count.ToString());
            EditorGUILayout.LabelField("Material count", report.Materials.Count.ToString());
            EditorGUILayout.LabelField("Texture count", report.Textures.Count.ToString());
            EditorGUILayout.LabelField("Texture memory estimate", LiteKitReportFormatter.FormatBytes(report.Stats.EstimatedTextureMemoryBytes));

            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Copy Markdown Report"))
                {
                    CopyToClipboard(LiteKitReportFormatter.ToMarkdown(report));
                }

                if (GUILayout.Button("Copy JSON Report"))
                {
                    CopyToClipboard(LiteKitReportFormatter.ToJson(report));
                }
            }
        }

        private void DrawSelectedTab()
        {
            if (report == null)
            {
                EditorGUILayout.LabelField("No report yet. Select an Avatar Root and click Analyze Avatar.");
                return;
            }

            switch (selectedTab)
            {
                case 0: DrawOverview(); break;
                case 1: DrawPerformance(); break;
                case 2: DrawRenderers(); break;
                case 3: DrawMaterials(); break;
                case 4: DrawTextures(); break;
                case 5: DrawMobileWarnings(); break;
                case 6: EditorGUILayout.TextArea(LiteKitReportFormatter.ToMarkdown(report)); break;
            }
        }

        private void DrawOverview()
        {
            EditorGUILayout.LabelField("Generated", report.GeneratedTime);
            EditorGUILayout.LabelField("Avatar", report.AvatarName);
            EditorGUILayout.LabelField("Platform", report.TargetPlatform.ToString());
            EditorGUILayout.LabelField("Estimated / Target", $"{report.EstimatedRank} / {report.TargetRank}");
            EditorGUILayout.LabelField("Triangles", report.Stats.TriangleCount.ToString());
            EditorGUILayout.LabelField("Skinned Meshes", report.Stats.SkinnedMeshRendererCount.ToString());
            EditorGUILayout.LabelField("Mesh Renderers", report.Stats.MeshRendererCount.ToString());
            EditorGUILayout.LabelField("Material Slots", report.Stats.MaterialSlotCount.ToString());
            EditorGUILayout.LabelField("Unique Materials", report.Stats.UniqueMaterialCount.ToString());
            EditorGUILayout.LabelField("Unique Textures", report.Stats.UniqueTextureCount.ToString());
            EditorGUILayout.LabelField("Texture Memory", LiteKitReportFormatter.FormatBytes(report.Stats.EstimatedTextureMemoryBytes));
            foreach (var warning in report.ScannerWarnings)
            {
                EditorGUILayout.HelpBox(warning, MessageType.Warning);
            }
        }

        private void DrawPerformance()
        {
            foreach (var metric in report.Evaluation.Metrics)
            {
                var messageType = metric.BlocksTarget ? MessageType.Warning : MessageType.None;
                EditorGUILayout.HelpBox($"{metric.DisplayName}: {metric.ValueText} / target {metric.TargetRank} <= {metric.TargetText} / estimated {metric.EstimatedRank}", messageType);
            }
        }

        private void DrawRenderers()
        {
            foreach (var item in report.Renderers)
            {
                EditorGUILayout.LabelField(item.Path, $"{item.RendererType}, slots {item.MaterialSlotCount}, mesh {item.MeshName}, tris {item.TriangleCount}, enabled {item.Enabled}, active {item.GameObjectActiveInHierarchy}, blendShapes {item.BlendShapeCount}, bones {item.BoneCount}");
            }
        }

        private void DrawMaterials()
        {
            EditorGUILayout.HelpBox("Mobile shader detection is a v0.1 heuristic: shader.name.StartsWith(\"VRChat/Mobile/\"). LiteKit does not convert materials.", MessageType.Info);
            foreach (var item in report.Materials)
            {
                EditorGUILayout.LabelField(item.Name, $"shader {item.ShaderName}, slots {item.SlotCount}, instancing {item.EnableGpuInstancing}, mobile {item.UsesMobileShader}, same-name candidates {item.HasSameNameCandidates}");
            }
        }

        private void DrawTextures()
        {
            foreach (var item in report.Textures)
            {
                EditorGUILayout.LabelField(item.Name, $"{item.Width}x{item.Height}, {item.TextureType}, mip {item.HasMipMap}, {LiteKitReportFormatter.FormatBytes(item.EstimatedMemoryBytes)}, type {item.ImporterTextureType}, max {item.ImporterMaxSize}, Android {item.AndroidOverride}, iOS {item.IosOverride}");
            }
        }

        private void DrawMobileWarnings()
        {
            EditorGUILayout.HelpBox("Warnings are advisory only. LiteKit v0.1 does not delete, disable, or convert components.", MessageType.Info);
            foreach (var warning in report.MobileWarnings)
            {
                EditorGUILayout.HelpBox($"[{warning.Category}] {warning.Path}: {warning.Message}", MessageType.Warning);
            }
        }

        private static void CopyToClipboard(string text)
        {
            EditorGUIUtility.systemCopyBuffer = text;
            LiteKitLogger.Info("Report copied to clipboard.");
        }
    }
}
