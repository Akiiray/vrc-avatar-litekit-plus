using System;
using System.Linq;
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

        private bool HasCurrentReport => report != null && report.Matches(avatarRoot, targetPlatform, targetRank);

        private static GameObject ResolveAvatar(GameObject selected)
        {
            var descriptor = selected != null ? selected.GetComponentInParent<VRCAvatarDescriptor>(true) : null;
            return descriptor != null ? descriptor.gameObject : selected;
        }

        private void UpdateReportValidity()
        {
            if (report != null && !report.Matches(avatarRoot, targetPlatform, targetRank)) report.IsStale = true;
        }

        private static MessageType ToMessageType(string severity) =>
            severity == "Error" ? MessageType.Error : severity == "Warning" ? MessageType.Warning : MessageType.Info;

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
            UpdateReportValidity();
            DrawStatusSection();
            if (HasCurrentReport)
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
                avatarRoot = ResolveAvatar((GameObject)EditorGUILayout.ObjectField(T(LiteKitTextKey.TargetAvatar), avatarRoot, typeof(GameObject), true));
                if (GUILayout.Button(T(LiteKitTextKey.UseHierarchySelection))) UseHierarchySelection();
                targetPlatform = (LiteKitPlatform)EditorGUILayout.EnumPopup(T(LiteKitTextKey.TargetPlatform), targetPlatform);
                var index = Mathf.Clamp(Array.IndexOf(TargetRankValues, targetRank), 0, TargetRankValues.Length - 1);
                index = EditorGUILayout.Popup(T(LiteKitTextKey.TargetRank), index, GetTargetRankLabels());
                targetRank = TargetRankValues[index];
                EditorGUILayout.Space(3f);
                UpdateReportValidity();
                if (GUILayout.Button(T(LiteKitTextKey.AnalyzeAvatar), GUILayout.Height(34f))) AnalyzeAvatar();
                if (avatarRoot == null) EditorGUILayout.HelpBox(T(LiteKitTextKey.NoAvatarSelected), MessageType.Warning);
            }
        }

        private string[] GetTargetRankLabels()
        {
            var suffix = T(LiteKitTextKey.RankSuffix);
            return new[] { "Excellent" + suffix, "Good" + suffix, "Medium" + suffix, "Poor" + suffix };
        }

        private void UseHierarchySelection()
        {
            var selected = Selection.activeGameObject;
            avatarRoot = ResolveAvatar(selected);
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

            avatarRoot = ResolveAvatar(avatarRoot);
            report = null;
            analysisState = AnalysisState.Error;
            LiteKitLogger.Info(string.Format(T(LiteKitTextKey.AnalysisStarted), avatarRoot.name));
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
                AnalyzedAvatar = context.AvatarRoot, AnalysisCompleted = true,
                GeneratedTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), AvatarName = avatarRoot.name,
                TargetPlatform = targetPlatform, TargetRank = targetRank, EstimatedRank = evaluation.EstimatedRank,
                Stats = stats, Evaluation = evaluation, Renderers = renderers, Materials = materials, Textures = textures,
                MobileWarnings = mobileWarnings, ScannerWarnings = new System.Collections.Generic.List<DiagnosticMessage>(context.Warnings),
                Notes = T(LiteKitTextKey.ReportNotes)
            };
            analysisState = mobileWarnings.Any(w => w.Severity == "Error") ? AnalysisState.Error
                : mobileWarnings.Any(w => w.Severity == "Warning") || context.Warnings.Count > 0 || evaluation.TargetPlan.BlockingMetrics.Count > 0
                    ? AnalysisState.Warning : AnalysisState.Success;
            LiteKitLogger.Info(string.Format(T(LiteKitTextKey.AnalysisCompleted), avatarRoot.name, evaluation.EstimatedRank));
        }

        private void DrawStatusSection()
        {
            using (new EditorGUILayout.VerticalScope("box"))
            {
                EditorGUILayout.LabelField(T(LiteKitTextKey.AnalysisStatus), EditorStyles.boldLabel);
                EditorGUILayout.LabelField(T(LiteKitTextKey.Status), GetStatusText());
                EditorGUILayout.LabelField(T(LiteKitTextKey.AvatarName), avatarRoot != null ? avatarRoot.name : "—");
                if (report != null && report.IsStale) EditorGUILayout.HelpBox(T(LiteKitTextKey.Stale), MessageType.Info);
            }
        }

        private string GetStatusText()
        {
            if (report != null && report.IsStale) return T(LiteKitTextKey.Stale);
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
                EditorGUILayout.HelpBox(string.Format(T(LiteKitTextKey.PerformanceLine), LiteKitLocalization.Label(language, metric.DisplayName),
                    LiteKitLocalization.Label(language, metric.ValueText), metric.TargetRank, LiteKitLocalization.MetricTarget(language, report.TargetPlatform, metric), metric.EstimatedRank),
                    metric.BlocksTarget ? MessageType.Warning : MessageType.None);
            }
            foreach (var warning in report.ScannerWarnings) EditorGUILayout.HelpBox(LiteKitLocalization.Diagnostic(language, warning), ToMessageType(warning.Severity));
        }

        private void DrawRenderers()
        {
            foreach (var item in report.Renderers)
                using (new EditorGUILayout.VerticalScope("box"))
                {
                    EditorGUILayout.LabelField(item.Path, EditorStyles.boldLabel);
                    EditorGUILayout.LabelField($"{T(LiteKitTextKey.Type)}: {item.RendererType}    {T(LiteKitTextKey.Mesh)}: {item.MeshName}");
                    EditorGUILayout.LabelField($"{T(LiteKitTextKey.Triangles)}: {item.TriangleCount:N0}    {T(LiteKitTextKey.MaterialSlots)}: {item.MaterialSlotCount}");
                    EditorGUILayout.LabelField($"{T(LiteKitTextKey.EnabledLabel)}: {LiteKitLocalization.Boolean(language, item.Enabled)}    {T(LiteKitTextKey.Active)}: {LiteKitLocalization.Boolean(language, item.GameObjectActiveInHierarchy)}    {T(LiteKitTextKey.BlendShapes)}: {item.BlendShapeCount}    {T(LiteKitTextKey.Bones)}: {item.BoneCount}");
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
                    EditorGUILayout.LabelField($"{T(LiteKitTextKey.MaterialSlots)}: {item.SlotCount}    {T(LiteKitTextKey.Instancing)}: {LiteKitLocalization.Boolean(language, item.EnableGpuInstancing)}");
                    EditorGUILayout.LabelField($"{T(LiteKitTextKey.MobileShader)}: {LiteKitLocalization.Boolean(language, item.UsesMobileShader)}    {T(LiteKitTextKey.SameNameCandidates)}: {LiteKitLocalization.Boolean(language, item.HasSameNameCandidates)}");
                }
        }

        private void DrawTextures()
        {
            foreach (var item in report.Textures)
                using (new EditorGUILayout.VerticalScope("box"))
                {
                    EditorGUILayout.LabelField(item.Name, EditorStyles.boldLabel);
                    EditorGUILayout.LabelField($"{T(LiteKitTextKey.Resolution)}: {item.Width}x{item.Height}    {T(LiteKitTextKey.TextureType)}: {item.TextureType}");
                    EditorGUILayout.LabelField($"{T(LiteKitTextKey.Mipmap)}: {LiteKitLocalization.Boolean(language, item.HasMipMap)}    {T(LiteKitTextKey.TextureMemory)}: {LiteKitReportFormatter.FormatBytes(item.EstimatedMemoryBytes)}");
                    EditorGUILayout.LabelField($"Importer: {item.ImporterTextureType}    {T(LiteKitTextKey.ImporterMaxSize)}: {item.ImporterMaxSize}");
                    EditorGUILayout.LabelField($"{T(LiteKitTextKey.AndroidOverride)}: {LiteKitLocalization.Boolean(language, item.AndroidOverride)}    {T(LiteKitTextKey.IosOverride)}: {LiteKitLocalization.Boolean(language, item.IosOverride)}");
                }
        }

        private void DrawMobileWarnings()
        {
            EditorGUILayout.HelpBox(T(LiteKitTextKey.MobileNotice), MessageType.Info);
            foreach (var warning in report.MobileWarnings)
                EditorGUILayout.HelpBox($"[{warning.Category}] {LiteKitLocalization.Diagnostic(language, warning)}", ToMessageType(warning.Severity));
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
