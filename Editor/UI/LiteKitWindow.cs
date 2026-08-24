using System;
using UnityEditor;
using UnityEngine;
using VRC.SDK3.Avatars.Components;
using Akiiray.VRCAvatarLiteKitPlus.Editor.Analyzer;
using Akiiray.VRCAvatarLiteKitPlus.Editor.Core;
using Akiiray.VRCAvatarLiteKitPlus.Editor.Localization;
using Akiiray.VRCAvatarLiteKitPlus.Editor.ParameterCompression;
using Akiiray.VRCAvatarLiteKitPlus.Editor.Planner;
using Akiiray.VRCAvatarLiteKitPlus.Editor.Utilities;

namespace Akiiray.VRCAvatarLiteKitPlus.Editor.UI
{
    public sealed class LiteKitWindow : EditorWindow
    {
        private enum AnalysisState { NotRun, Success, Warning, Error }

        private static readonly PerformanceRank[] TargetRankValues =
            { PerformanceRank.Excellent, PerformanceRank.Good, PerformanceRank.Medium, PerformanceRank.Poor };

        private GameObject avatarRoot;
        private LiteKitPlatform targetPlatform = LiteKitPlatform.PC;
        private PerformanceRank targetRank = PerformanceRank.Poor;
        private LiteKitReport report;
        private LiteKitLanguage language = LiteKitLanguage.Japanese;
        private AnalysisState analysisState;
        private Vector2 mainScroll;
        private Vector2 rawReportScroll;
        private bool performanceFoldout;
        private bool rendererFoldout;
        private bool materialFoldout;
        private bool textureFoldout;
        private bool mobileFoldout;
        private bool rawReportFoldout;

        [MenuItem(LiteKitConstants.MenuRoot + "/日本語ウィンドウ", false, 0)]
        private static void OpenJapanese() => OpenWithLanguage(LiteKitLanguage.Japanese);

        [MenuItem(LiteKitConstants.MenuRoot + "/English Window", false, 1)]
        private static void OpenEnglish() => OpenWithLanguage(LiteKitLanguage.English);

        public static void Open() => OpenWithLanguage(LiteKitLanguage.Japanese);

        private static void OpenWithLanguage(LiteKitLanguage selectedLanguage)
        {
            var window = GetWindow<LiteKitWindow>(LiteKitConstants.ToolName);
            window.language = selectedLanguage;
            LiteKitLocalization.CurrentLanguage = selectedLanguage;
            window.minSize = new Vector2(520f, 620f);
            window.titleContent = new GUIContent(LiteKitConstants.ToolName);
            window.Show();
            window.Repaint();
        }

        private string T(LiteKitTextKey key) => LiteKitLocalization.Get(language, key);

        private void OnEnable()
        {
            minSize = new Vector2(520f, 620f);
            titleContent = new GUIContent(LiteKitConstants.ToolName);
            language = LiteKitLocalization.CurrentLanguage;
        }

        private void OnGUI()
        {
            mainScroll = EditorGUILayout.BeginScrollView(mainScroll);
            EditorGUILayout.LabelField(T(LiteKitTextKey.WindowTitle), EditorStyles.boldLabel);
            EditorGUILayout.HelpBox(T(LiteKitTextKey.DryRunNotice), MessageType.Info);
            EditorGUILayout.Space();
            DrawTargetSection();
            DrawStatusSection();
            if (report != null)
            {
                DrawSummarySection();
                DrawParameterSection();
                DrawDetailSections();
            }
            EditorGUILayout.EndScrollView();
        }

        private void DrawTargetSection()
        {
            using (new EditorGUILayout.VerticalScope("box"))
            {
                EditorGUILayout.LabelField(T(LiteKitTextKey.TargetSection), EditorStyles.boldLabel);
                avatarRoot = (GameObject)EditorGUILayout.ObjectField(T(LiteKitTextKey.TargetAvatar), avatarRoot, typeof(GameObject), true);
                if (GUILayout.Button(T(LiteKitTextKey.UseHierarchySelection))) UseHierarchySelection();
                targetPlatform = (LiteKitPlatform)EditorGUILayout.EnumPopup(T(LiteKitTextKey.TargetPlatform), targetPlatform);
                var index = Mathf.Clamp(Array.IndexOf(TargetRankValues, targetRank), 0, TargetRankValues.Length - 1);
                index = EditorGUILayout.Popup(T(LiteKitTextKey.TargetRank), index, GetTargetRankLabels());
                targetRank = TargetRankValues[index];
                EditorGUILayout.Space(3f);
                if (GUILayout.Button(T(LiteKitTextKey.AnalyzeAvatar), GUILayout.Height(34f))) AnalyzeAvatar();
                if (avatarRoot == null) EditorGUILayout.HelpBox(T(LiteKitTextKey.NoAvatarSelected), MessageType.Warning);
            }
        }

        private string[] GetTargetRankLabels()
        {
            var suffix = language == LiteKitLanguage.Japanese ? "以上" : " or better";
            return new[] { "Excellent" + suffix, "Good" + suffix, "Medium" + suffix, "Poor" + suffix };
        }

        private void UseHierarchySelection()
        {
            var selected = Selection.activeGameObject;
            var descriptor = selected != null ? selected.GetComponentInParent<VRCAvatarDescriptor>(true) : null;
            avatarRoot = descriptor != null ? descriptor.gameObject : selected;
            if (avatarRoot == null) analysisState = AnalysisState.Error;
        }

        private void AnalyzeAvatar()
        {
            if (avatarRoot == null)
            {
                LiteKitLogger.Warning(T(LiteKitTextKey.NoAvatarSelected));
                report = null;
                analysisState = AnalysisState.Error;
                return;
            }

            var descriptor = avatarRoot.GetComponentInParent<VRCAvatarDescriptor>(true);
            if (descriptor != null) avatarRoot = descriptor.gameObject;
            LiteKitLogger.Info($"Analyze started: {avatarRoot.name}");
            var context = AvatarScanner.Scan(avatarRoot);
            if (context == null || !context.IsValid)
            {
                report = null;
                analysisState = AnalysisState.Error;
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
                GeneratedTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), AvatarName = avatarRoot.name,
                TargetPlatform = targetPlatform, TargetRank = targetRank, EstimatedRank = evaluation.EstimatedRank,
                Stats = stats, Evaluation = evaluation, Renderers = renderers, Materials = materials, Textures = textures,
                MobileWarnings = mobileWarnings, ScannerWarnings = new System.Collections.Generic.List<string>(context.Warnings),
                Notes = language == LiteKitLanguage.Japanese
                    ? "非破壊のDry Runレポートです。元のMaterial、Texture、Mesh、Prefab、Avatar Assetは変更していません。"
                    : "This is a non-destructive Dry Run report. No original Material, Texture, Mesh, Prefab, or Avatar asset was modified."
            };
            analysisState = mobileWarnings.Count > 0 || context.Warnings.Count > 0 || evaluation.TargetPlan.BlockingMetrics.Count > 0
                ? AnalysisState.Warning : AnalysisState.Success;
            LiteKitLogger.Info($"Analyze completed: {avatarRoot.name}; Estimated Rank: {evaluation.EstimatedRank}");
        }

        private void DrawStatusSection()
        {
            using (new EditorGUILayout.VerticalScope("box"))
            {
                EditorGUILayout.LabelField(T(LiteKitTextKey.AnalysisStatus), EditorStyles.boldLabel);
                EditorGUILayout.LabelField(T(LiteKitTextKey.Status), GetStatusText());
                EditorGUILayout.LabelField(T(LiteKitTextKey.AvatarName), avatarRoot != null ? avatarRoot.name : "—");
            }
        }

        private string GetStatusText()
        {
            switch (analysisState)
            {
                case AnalysisState.Success: return T(LiteKitTextKey.Completed);
                case AnalysisState.Warning: return T(LiteKitTextKey.Warnings);
                case AnalysisState.Error: return T(LiteKitTextKey.Error);
                default: return T(LiteKitTextKey.NotRun);
            }
        }

        private void DrawSummarySection()
        {
            using (new EditorGUILayout.VerticalScope("box"))
            {
                EditorGUILayout.LabelField(T(LiteKitTextKey.Summary), EditorStyles.boldLabel);
                var columns = position.width >= 650f ? 3 : 2;
                DrawSummaryCards(columns,
                    new SummaryItem(T(LiteKitTextKey.EstimatedRank), report.EstimatedRank.ToString()),
                    new SummaryItem(T(LiteKitTextKey.TargetRank), GetTargetRankLabels()[Array.IndexOf(TargetRankValues, report.TargetRank)]),
                    new SummaryItem(T(LiteKitTextKey.BlockingMetrics), report.Evaluation.TargetPlan.BlockingMetrics.Count.ToString()),
                    new SummaryItem(T(LiteKitTextKey.Triangles), report.Stats.TriangleCount.ToString("N0")),
                    new SummaryItem(T(LiteKitTextKey.RendererCount), report.Renderers.Count.ToString()),
                    new SummaryItem(T(LiteKitTextKey.MaterialCount), report.Materials.Count.ToString()),
                    new SummaryItem(T(LiteKitTextKey.TextureCount), report.Textures.Count.ToString()),
                    new SummaryItem(T(LiteKitTextKey.TextureMemory), LiteKitReportFormatter.FormatBytes(report.Stats.EstimatedTextureMemoryBytes)));
            }
        }

        private struct SummaryItem { internal readonly string Label, Value; internal SummaryItem(string label, string value) { Label = label; Value = value; } }

        private static void DrawSummaryCards(int columns, params SummaryItem[] items)
        {
            for (var i = 0; i < items.Length; i += columns)
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    for (var j = 0; j < columns; j++)
                    {
                        if (i + j >= items.Length) { GUILayout.FlexibleSpace(); continue; }
                        using (new EditorGUILayout.VerticalScope("box", GUILayout.ExpandWidth(true)))
                        {
                            EditorGUILayout.LabelField(items[i + j].Label, EditorStyles.miniLabel);
                            EditorGUILayout.LabelField(items[i + j].Value, EditorStyles.boldLabel);
                        }
                    }
                }
            }
        }

        private void DrawParameterSection()
        {
            using (new EditorGUILayout.VerticalScope("box"))
            {
                EditorGUILayout.LabelField(T(LiteKitTextKey.ParameterSection), EditorStyles.boldLabel);
                var compressor = avatarRoot != null ? avatarRoot.GetComponent<LiteKitParameterCompressor>() : null;
                var state = compressor == null ? T(LiteKitTextKey.NotAdded)
                    : compressor.EnableCompression ? $"{T(LiteKitTextKey.Enabled)} (K={compressor.SyncChannelCount})" : T(LiteKitTextKey.Disabled);
                EditorGUILayout.LabelField(T(LiteKitTextKey.ParameterCompressor), state);
            }
        }

        private void DrawDetailSections()
        {
            performanceFoldout = DrawFoldout(performanceFoldout, T(LiteKitTextKey.PerformanceDetails), report.Evaluation.Metrics.Count, DrawPerformance);
            rendererFoldout = DrawFoldout(rendererFoldout, T(LiteKitTextKey.RendererDetails), report.Renderers.Count, DrawRenderers);
            materialFoldout = DrawFoldout(materialFoldout, T(LiteKitTextKey.MaterialDetails), report.Materials.Count, DrawMaterials);
            textureFoldout = DrawFoldout(textureFoldout, T(LiteKitTextKey.TextureDetails), report.Textures.Count, DrawTextures);
            mobileFoldout = DrawFoldout(mobileFoldout, T(LiteKitTextKey.MobileWarnings), report.MobileWarnings.Count, DrawMobileWarnings);
            rawReportFoldout = DrawFoldout(rawReportFoldout, T(LiteKitTextKey.RawReport), -1, DrawRawReport);
        }

        private static bool DrawFoldout(bool value, string title, int count, Action content)
        {
            using (new EditorGUILayout.VerticalScope("box"))
            {
                value = EditorGUILayout.Foldout(value, count >= 0 ? $"{title} ({count})" : title, true, EditorStyles.foldoutHeader);
                if (value) content();
            }
            return value;
        }

        private void DrawPerformance()
        {
            foreach (var metric in report.Evaluation.Metrics)
            {
                var targetWord = language == LiteKitLanguage.Japanese ? "目標" : "target";
                var estimatedWord = language == LiteKitLanguage.Japanese ? "推定" : "estimated";
                EditorGUILayout.HelpBox($"{metric.DisplayName}: {metric.ValueText}\n{targetWord} {metric.TargetRank}: {metric.TargetText}以下 / {estimatedWord} {metric.EstimatedRank}", metric.BlocksTarget ? MessageType.Warning : MessageType.None);
            }
            foreach (var warning in report.ScannerWarnings) EditorGUILayout.HelpBox(warning, MessageType.Warning);
        }

        private void DrawRenderers()
        {
            foreach (var item in report.Renderers)
                using (new EditorGUILayout.VerticalScope("box"))
                {
                    EditorGUILayout.LabelField(item.Path, EditorStyles.boldLabel);
                    EditorGUILayout.LabelField($"{T(LiteKitTextKey.Type)}: {item.RendererType}    {T(LiteKitTextKey.Mesh)}: {item.MeshName}");
                    EditorGUILayout.LabelField($"Triangles: {item.TriangleCount:N0}    {T(LiteKitTextKey.MaterialSlots)}: {item.MaterialSlotCount}");
                    EditorGUILayout.LabelField($"{T(LiteKitTextKey.EnabledLabel)}: {item.Enabled}    {T(LiteKitTextKey.Active)}: {item.GameObjectActiveInHierarchy}    {T(LiteKitTextKey.BlendShapes)}: {item.BlendShapeCount}    {T(LiteKitTextKey.Bones)}: {item.BoneCount}");
                }
        }

        private void DrawMaterials()
        {
            EditorGUILayout.HelpBox(T(LiteKitTextKey.MaterialNotice), MessageType.Info);
            foreach (var item in report.Materials)
                using (new EditorGUILayout.VerticalScope("box"))
                {
                    EditorGUILayout.LabelField(item.Name, EditorStyles.boldLabel);
                    EditorGUILayout.LabelField($"{T(LiteKitTextKey.Shader)}: {item.ShaderName}");
                    EditorGUILayout.LabelField($"{T(LiteKitTextKey.MaterialSlots)}: {item.SlotCount}    {T(LiteKitTextKey.Instancing)}: {item.EnableGpuInstancing}");
                    EditorGUILayout.LabelField($"{T(LiteKitTextKey.MobileShader)}: {item.UsesMobileShader}    {T(LiteKitTextKey.SameNameCandidates)}: {item.HasSameNameCandidates}");
                }
        }

        private void DrawTextures()
        {
            foreach (var item in report.Textures)
                using (new EditorGUILayout.VerticalScope("box"))
                {
                    EditorGUILayout.LabelField(item.Name, EditorStyles.boldLabel);
                    EditorGUILayout.LabelField($"{T(LiteKitTextKey.Resolution)}: {item.Width}x{item.Height}    {T(LiteKitTextKey.TextureType)}: {item.TextureType}");
                    EditorGUILayout.LabelField($"{T(LiteKitTextKey.Mipmap)}: {item.HasMipMap}    {T(LiteKitTextKey.TextureMemory)}: {LiteKitReportFormatter.FormatBytes(item.EstimatedMemoryBytes)}");
                    EditorGUILayout.LabelField($"Importer: {item.ImporterTextureType}    {T(LiteKitTextKey.ImporterMaxSize)}: {item.ImporterMaxSize}");
                    EditorGUILayout.LabelField($"{T(LiteKitTextKey.AndroidOverride)}: {item.AndroidOverride}    {T(LiteKitTextKey.IosOverride)}: {item.IosOverride}");
                }
        }

        private void DrawMobileWarnings()
        {
            EditorGUILayout.HelpBox(T(LiteKitTextKey.MobileNotice), MessageType.Info);
            foreach (var warning in report.MobileWarnings)
                EditorGUILayout.HelpBox($"[{warning.Category}] {warning.Path}: {warning.Message}", MessageType.Warning);
        }

        private void DrawRawReport()
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button(T(LiteKitTextKey.CopyMarkdown))) CopyToClipboard(LiteKitReportFormatter.ToMarkdown(report, language));
                if (GUILayout.Button(T(LiteKitTextKey.CopyJson))) CopyToClipboard(LiteKitReportFormatter.ToJson(report));
            }
            rawReportScroll = EditorGUILayout.BeginScrollView(rawReportScroll, GUILayout.MinHeight(180f), GUILayout.MaxHeight(360f));
            EditorGUILayout.TextArea(LiteKitReportFormatter.ToMarkdown(report, language), GUILayout.ExpandHeight(true));
            EditorGUILayout.EndScrollView();
        }

        private void CopyToClipboard(string text)
        {
            EditorGUIUtility.systemCopyBuffer = text;
            LiteKitLogger.Info(T(LiteKitTextKey.Copied));
        }
    }
}
