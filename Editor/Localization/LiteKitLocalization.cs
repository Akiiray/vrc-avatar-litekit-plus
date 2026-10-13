using System.Collections.Generic;
using UnityEditor;
using Akiiray.VRCAvatarLiteKitPlus.Editor.Analyzer;
using Akiiray.VRCAvatarLiteKitPlus.Editor.Planner;
using Akiiray.VRCAvatarLiteKitPlus.Editor.Core;

namespace Akiiray.VRCAvatarLiteKitPlus.Editor.Localization
{
    internal static class LiteKitLocalization
    {
        internal const string LanguagePreferenceKey = "Akiiray.VRCAvatarLiteKitPlus.Language";

        private static readonly Dictionary<LiteKitTextKey, string> Japanese = new Dictionary<LiteKitTextKey, string>
        {
            { LiteKitTextKey.WindowTitle, "VRC Avatar LiteKit Plus" },
            { LiteKitTextKey.DryRunNotice, "この診断は非破壊のDry Runです。\n元のMaterial、Texture、Mesh、Prefab、Avatar Assetは変更しません。\n\nPerformance RankはVRChat公式閾値を基にした推定であり、\nVRChat SDK BuilderのValidation結果と完全一致しない場合があります。" },
            { LiteKitTextKey.TargetSection, "対象アバター・診断" }, { LiteKitTextKey.TargetAvatar, "対象アバター" },
            { LiteKitTextKey.UseHierarchySelection, "選択中のHierarchyから設定" }, { LiteKitTextKey.TargetPlatform, "対象Platform" },
            { LiteKitTextKey.TargetRank, "目標Performance Rank" }, { LiteKitTextKey.AnalyzeAvatar, "アバターを診断" },
            { LiteKitTextKey.NoAvatarSelected, "対象アバターが選択されていません。\nHierarchyからAvatarを選択してください。" },
            { LiteKitTextKey.AnalysisStatus, "診断状態" }, { LiteKitTextKey.Status, "状態" }, { LiteKitTextKey.NotRun, "未実行" },
            { LiteKitTextKey.Completed, "✓ 完了" }, { LiteKitTextKey.Warnings, "! 警告あり" }, { LiteKitTextKey.Error, "! エラー" },
            { LiteKitTextKey.AvatarName, "対象Avatar名" }, { LiteKitTextKey.Summary, "診断サマリー" },
            { LiteKitTextKey.EstimatedRank, "推定Performance Rank" }, { LiteKitTextKey.BlockingMetrics, "目標達成を妨げる項目" },
            { LiteKitTextKey.Triangles, "ポリゴン数" }, { LiteKitTextKey.RendererCount, "Renderer数" }, { LiteKitTextKey.MaterialCount, "Material数" },
            { LiteKitTextKey.TextureCount, "Texture数" }, { LiteKitTextKey.TextureMemory, "推定Texture Memory" },
            { LiteKitTextKey.ParameterSection, "Parameter / 最適化状態" }, { LiteKitTextKey.ParameterCompressor, "Parameter Compressor" },
            { LiteKitTextKey.NotAdded, "未導入" }, { LiteKitTextKey.Enabled, "有効" }, { LiteKitTextKey.Disabled, "無効" },
            { LiteKitTextKey.PerformanceDetails, "Performance詳細" }, { LiteKitTextKey.RendererDetails, "Renderer詳細" },
            { LiteKitTextKey.MaterialDetails, "Material詳細" }, { LiteKitTextKey.TextureDetails, "Texture詳細" },
            { LiteKitTextKey.MobileWarnings, "Mobile警告" }, { LiteKitTextKey.RawReport, "詳細レポート" },
            { LiteKitTextKey.CopyMarkdown, "Markdownをコピー" }, { LiteKitTextKey.CopyJson, "JSONをコピー" },
            { LiteKitTextKey.PreviewDisclaimer, "プレビュー値はDescriptorのExpression ParametersとFX AnimatorControllerに基づく推定です。\nBuild時の最終Parameter構成によって結果が異なる場合があります。" },
            { LiteKitTextKey.MaterialNotice, "Mobile Shader判定は簡易判定です。\n現時点ではMaterial変換は行いません。" },
            { LiteKitTextKey.MobileNotice, "この警告は診断情報です。\nLiteKitはこの診断によってComponentの削除、無効化、変換を行いません。" },
            { LiteKitTextKey.Generated, "生成日時" }, { LiteKitTextKey.Platform, "Platform" }, { LiteKitTextKey.SkinnedMeshes, "Skinned Mesh" },
            { LiteKitTextKey.MeshRenderers, "Mesh Renderer" }, { LiteKitTextKey.MaterialSlots, "Materialスロット数" },
            { LiteKitTextKey.UniqueMaterials, "異なるMaterial数" }, { LiteKitTextKey.UniqueTextures, "異なるTexture数" },
            { LiteKitTextKey.Path, "パス" }, { LiteKitTextKey.Type, "種類" }, { LiteKitTextKey.Mesh, "Mesh" },
            { LiteKitTextKey.EnabledLabel, "有効" }, { LiteKitTextKey.Active, "アクティブ" }, { LiteKitTextKey.BlendShapes, "BlendShapes" },
            { LiteKitTextKey.Bones, "ボーン数" }, { LiteKitTextKey.Shader, "Shader" }, { LiteKitTextKey.Instancing, "GPU Instancing" },
            { LiteKitTextKey.MobileShader, "Mobile Shader" }, { LiteKitTextKey.SameNameCandidates, "同名候補" },
            { LiteKitTextKey.Resolution, "解像度" }, { LiteKitTextKey.Mipmap, "Mipmap" }, { LiteKitTextKey.TextureType, "Texture種別" },
            { LiteKitTextKey.ImporterMaxSize, "Importer最大サイズ" }, { LiteKitTextKey.AndroidOverride, "Android個別設定" },
            { LiteKitTextKey.IosOverride, "iOS個別設定" }, { LiteKitTextKey.Copied, "レポートをクリップボードへコピーしました。" },
            { LiteKitTextKey.Stale, "対象または診断条件が変更されました。再診断してください。" },
            { LiteKitTextKey.AnalysisStarted, "診断開始: {0}" },
            { LiteKitTextKey.AnalysisCompleted, "診断完了: {0}; 推定Rank: {1}" },
            { LiteKitTextKey.ReportNotes, "非破壊のDry Runレポートです。元のMaterial、Texture、Mesh、Prefab、Avatar Assetは変更していません。" },
            { LiteKitTextKey.PerformanceLine, "{0}: {1}\n目標 {2}: {3} / 推定 {4}" },
            { LiteKitTextKey.RankSuffix, "以上" },
            { LiteKitTextKey.CompressionNotice, "NDMF Build時に同期Float / Intを共有チャンネルで時分割送信します。元アセットは変更しません。" },
            { LiteKitTextKey.EnableCompression, "圧縮を有効にする" },
            { LiteKitTextKey.SyncChannelCount, "同期チャンネル数" },
            { LiteKitTextKey.ChannelNotice, "リモートへの同時送信数です。増やすと反映が速くなりますが、bit消費が増えます。" },
            { LiteKitTextKey.DescriptorRequired, "このComponentはVRCAvatarDescriptorがあるAvatar Rootで使用してください。" },
            { LiteKitTextKey.Parameters, "Parameter一覧" },
            { LiteKitTextKey.Rescan, "再スキャン" },
            { LiteKitTextKey.CompressionPreview, "圧縮結果プレビュー" },
            { LiteKitTextKey.CompressionTargets, "対象: Float {0}個 + Int {1}個 = {2}個" },
            { LiteKitTextKey.NoCompressionTargets, "圧縮対象の同期Float / Int Parameterはありません。" },
            { LiteKitTextKey.CompressionBefore, "圧縮前: {0}bit" },
            { LiteKitTextKey.CompressionAfter, "圧縮後: {0}bit" },
            { LiteKitTextKey.CompressionSaved, "節約: {0}bit ({1:0.0}%)" },
            { LiteKitTextKey.CompressionBatches, "同期チャンネル: {0} ch / Batch数: {1}" },
            { LiteKitTextKey.CompressionCycle, "同期遅延（最大一周）: 約 {0:0.00}秒" },
            { LiteKitTextKey.NoSavings, "圧縮によるbit削減効果がないため、Build時は処理しません。" },
            { LiteKitTextKey.BatchLimit, "Parameter Compressorで扱えるBatch数の上限を超えています。同期チャンネル数を増やすか、圧縮対象を減らしてください。" },
            { LiteKitTextKey.HighLatency, "同期遅延が大きくなります。リモート側への反映に影響する可能性があります。" },
            { LiteKitTextKey.ChannelLatencyHint, "同期チャンネル数を増やすと遅延を短縮できます。" },
            { LiteKitTextKey.SavedParameterNotice, "saved Parameterが含まれています。ワールド移動やAvatar読み込み直後は、リモート側への値の再同期に時間がかかる場合があります。saved設定自体は維持されます。" },
            { LiteKitTextKey.ParameterBudget, "Parameter使用量 (256bit)" },
            { LiteKitTextKey.FinalBudget, "圧縮後 {0}bit / 256bit（削減 {1}bit）" },
            { LiteKitTextKey.BudgetExceeded, "圧縮後もParameter予算 256bit を超過しています。" },
            { LiteKitTextKey.BudgetLegend, "  非圧縮（青） / 圧縮Channel（橙） / 空き" },
            { LiteKitTextKey.Synced, "同期" },
            { LiteKitTextKey.LocalParameter, "ローカル" },
            { LiteKitTextKey.Saved, "保存" },
            { LiteKitTextKey.NotSaved, "未保存" },
            { LiteKitTextKey.Excluded, "圧縮除外" },
            { LiteKitTextKey.Included, "圧縮対象" },
            { LiteKitTextKey.MenuBuilderNotice, "MenuBuilderの圧縮機能が検出されました。Parameter Compressorとの併用は推奨されません。MenuBuilder側の圧縮設定をOFFにしてください。" },
            { LiteKitTextKey.VrcFuryNotice, "VRCFuryの同期用と思われるParameterを検出しました。これらは圧縮対象から自動的に除外します。" },
            { LiteKitTextKey.FaceTrackingNotice, "Face Tracking Parameterが検出されました。高頻度で更新されるため、Parameter Compressorの圧縮対象から自動的に除外されます。" },
            { LiteKitTextKey.ReportTitle, "# VRC Avatar LiteKit Plus 診断レポート" },
            { LiteKitTextKey.ReportMetadata, "- 生成日時: {0}\n- Avatar: {1}\n- 対象Platform: {2}\n- 目標Performance Rank: {3}以上\n- 推定Performance Rank: {4}" },
            { LiteKitTextKey.ReportDisclaimer, "> この結果はVRChat公式Performance Rank閾値を基にした推定です。\n> VRChat SDK BuilderのValidation結果と完全一致しない場合があります。" },
            { LiteKitTextKey.ReportSummary, "## 概要" },
            { LiteKitTextKey.ReportPerformance, "## Performance評価" },
            { LiteKitTextKey.ReportRenderers, "## Renderer一覧" },
            { LiteKitTextKey.ReportMaterials, "## Material一覧" },
            { LiteKitTextKey.ReportTextures, "## Texture一覧" },
            { LiteKitTextKey.ReportScanner, "## Scanner警告" },
            { LiteKitTextKey.ReportMobile, "## Mobile警告" },
            { LiteKitTextKey.ReportNotesHeader, "## 注記" },
            { LiteKitTextKey.ReportSummaryLine, "- ポリゴン数: {0}\n- Renderer: {1}\n- Material: {2}\n- Texture: {3}\n- 推定Texture Memory: {4}\n- Mobile警告: {5}" },
            { LiteKitTextKey.ReportMetric, "- {0}: {1} / 目標 {2}: {3} / 推定 {4}{5}" },
            { LiteKitTextKey.ReportRenderer, "- {0}: {1}, Materialスロット {2}, Mesh {3}, ポリゴン数 {4}, 有効 {5}, アクティブ {6}" },
            { LiteKitTextKey.ReportMaterial, "- {0}: Shader {1}, スロット {2}, Mobile Shader {3}, GPU Instancing {4}" },
            { LiteKitTextKey.ReportTexture, "- {0}: {1}x{2}, {3}, {4}, Importer {5}, 最大サイズ {6}, Android {7}, iOS {8}" },
        };

        private static readonly Dictionary<LiteKitTextKey, string> English = new Dictionary<LiteKitTextKey, string>
        {
            { LiteKitTextKey.WindowTitle, "VRC Avatar LiteKit Plus" },
            { LiteKitTextKey.DryRunNotice, "This analysis is a non-destructive Dry Run.\nIt does not modify the original Materials, Textures, Meshes, Prefabs, or Avatar assets.\n\nPerformance Rank is an estimate based on public VRChat thresholds\nand may not exactly match VRChat SDK Builder validation." },
            { LiteKitTextKey.TargetSection, "Target Avatar & Analysis" }, { LiteKitTextKey.TargetAvatar, "Target Avatar" },
            { LiteKitTextKey.UseHierarchySelection, "Use Current Hierarchy Selection" }, { LiteKitTextKey.TargetPlatform, "Target Platform" },
            { LiteKitTextKey.TargetRank, "Target Performance Rank" }, { LiteKitTextKey.AnalyzeAvatar, "Analyze Avatar" },
            { LiteKitTextKey.NoAvatarSelected, "No target avatar is selected.\nSelect an avatar from the Hierarchy." },
            { LiteKitTextKey.AnalysisStatus, "Analysis Status" }, { LiteKitTextKey.Status, "Status" }, { LiteKitTextKey.NotRun, "Not Run" },
            { LiteKitTextKey.Completed, "✓ Completed" }, { LiteKitTextKey.Warnings, "! Warnings" }, { LiteKitTextKey.Error, "! Error" },
            { LiteKitTextKey.AvatarName, "Target Avatar" }, { LiteKitTextKey.Summary, "Analysis Summary" },
            { LiteKitTextKey.EstimatedRank, "Estimated Performance Rank" }, { LiteKitTextKey.BlockingMetrics, "Blocking Metrics" },
            { LiteKitTextKey.Triangles, "Triangles" }, { LiteKitTextKey.RendererCount, "Renderers" }, { LiteKitTextKey.MaterialCount, "Materials" },
            { LiteKitTextKey.TextureCount, "Textures" }, { LiteKitTextKey.TextureMemory, "Estimated Texture Memory" },
            { LiteKitTextKey.ParameterSection, "Parameter / Optimization" }, { LiteKitTextKey.ParameterCompressor, "Parameter Compressor" },
            { LiteKitTextKey.NotAdded, "Not Added" }, { LiteKitTextKey.Enabled, "Enabled" }, { LiteKitTextKey.Disabled, "Disabled" },
            { LiteKitTextKey.PerformanceDetails, "Performance Details" }, { LiteKitTextKey.RendererDetails, "Renderer Details" },
            { LiteKitTextKey.MaterialDetails, "Material Details" }, { LiteKitTextKey.TextureDetails, "Texture Details" },
            { LiteKitTextKey.MobileWarnings, "Mobile Warnings" }, { LiteKitTextKey.RawReport, "Raw Report" },
            { LiteKitTextKey.CopyMarkdown, "Copy Markdown" }, { LiteKitTextKey.CopyJson, "Copy JSON" },
            { LiteKitTextKey.PreviewDisclaimer, "Preview values are estimates from the Descriptor’s Expression Parameters and FX AnimatorController.\nThe final result may differ during the avatar build." },
            { LiteKitTextKey.MaterialNotice, "Mobile Shader detection is currently heuristic.\nLiteKit does not convert Materials in this analysis." },
            { LiteKitTextKey.MobileNotice, "These warnings are advisory.\nLiteKit does not delete, disable, or convert components as part of this analysis." },
            { LiteKitTextKey.Generated, "Generated" }, { LiteKitTextKey.Platform, "Platform" }, { LiteKitTextKey.SkinnedMeshes, "Skinned Meshes" },
            { LiteKitTextKey.MeshRenderers, "Mesh Renderers" }, { LiteKitTextKey.MaterialSlots, "Material Slots" },
            { LiteKitTextKey.UniqueMaterials, "Unique Materials" }, { LiteKitTextKey.UniqueTextures, "Unique Textures" },
            { LiteKitTextKey.Path, "Path" }, { LiteKitTextKey.Type, "Type" }, { LiteKitTextKey.Mesh, "Mesh" },
            { LiteKitTextKey.EnabledLabel, "Enabled" }, { LiteKitTextKey.Active, "Active" }, { LiteKitTextKey.BlendShapes, "BlendShapes" },
            { LiteKitTextKey.Bones, "Bones" }, { LiteKitTextKey.Shader, "Shader" }, { LiteKitTextKey.Instancing, "GPU Instancing" },
            { LiteKitTextKey.MobileShader, "Mobile Shader" }, { LiteKitTextKey.SameNameCandidates, "Same-name Candidates" },
            { LiteKitTextKey.Resolution, "Resolution" }, { LiteKitTextKey.Mipmap, "Mipmap" }, { LiteKitTextKey.TextureType, "Texture Type" },
            { LiteKitTextKey.ImporterMaxSize, "Importer Max Size" }, { LiteKitTextKey.AndroidOverride, "Android Override" },
            { LiteKitTextKey.IosOverride, "iOS Override" }, { LiteKitTextKey.Copied, "Report copied to the clipboard." },
            { LiteKitTextKey.Stale, "The target or analysis settings have changed. Run analysis again." },
            { LiteKitTextKey.AnalysisStarted, "Analysis started: {0}" },
            { LiteKitTextKey.AnalysisCompleted, "Analysis completed: {0}; Estimated Rank: {1}" },
            { LiteKitTextKey.ReportNotes, "This is a non-destructive Dry Run report. No original Material, Texture, Mesh, Prefab, or Avatar asset was modified." },
            { LiteKitTextKey.PerformanceLine, "{0}: {1}\nTarget {2}: {3} / Estimated {4}" },
            { LiteKitTextKey.RankSuffix, " or better" },
            { LiteKitTextKey.CompressionNotice, "During an NDMF build, synchronized Float / Int parameters are time-multiplexed over shared channels. Original assets are not modified." },
            { LiteKitTextKey.EnableCompression, "Enable Compression" },
            { LiteKitTextKey.SyncChannelCount, "Sync Channel Count" },
            { LiteKitTextKey.ChannelNotice, "The number of values sent to remote users at once. More channels update faster but consume more bits." },
            { LiteKitTextKey.DescriptorRequired, "Use this component on an Avatar Root with a VRCAvatarDescriptor." },
            { LiteKitTextKey.Parameters, "Parameters" },
            { LiteKitTextKey.Rescan, "Rescan" },
            { LiteKitTextKey.CompressionPreview, "Compression Preview" },
            { LiteKitTextKey.CompressionTargets, "Targets: {0} Float + {1} Int = {2}" },
            { LiteKitTextKey.NoCompressionTargets, "There are no eligible synchronized Float / Int parameters." },
            { LiteKitTextKey.CompressionBefore, "Before: {0} bits" },
            { LiteKitTextKey.CompressionAfter, "After: {0} bits" },
            { LiteKitTextKey.CompressionSaved, "Saved: {0} bits ({1:0.0}%)" },
            { LiteKitTextKey.CompressionBatches, "Sync Channels: {0} / Batches: {1}" },
            { LiteKitTextKey.CompressionCycle, "Maximum Sync Cycle: about {0:0.00} seconds" },
            { LiteKitTextKey.NoSavings, "No bit savings; compression will be skipped during the build." },
            { LiteKitTextKey.BatchLimit, "The batch limit is exceeded. Increase channels or reduce compression targets." },
            { LiteKitTextKey.HighLatency, "High sync latency may delay updates for remote users." },
            { LiteKitTextKey.ChannelLatencyHint, "More sync channels can reduce latency." },
            { LiteKitTextKey.SavedParameterNotice, "Saved parameters are included. Remote resynchronization may take time after world changes or avatar loading. The saved settings are preserved." },
            { LiteKitTextKey.ParameterBudget, "Parameter Usage (256 bits)" },
            { LiteKitTextKey.FinalBudget, "After: {0} / 256 bits (saved: {1} bits)" },
            { LiteKitTextKey.BudgetExceeded, "The compressed parameters still exceed the 256-bit budget." },
            { LiteKitTextKey.BudgetLegend, "  Uncompressed (blue) / Channels (orange) / Free" },
            { LiteKitTextKey.Synced, "Synced" },
            { LiteKitTextKey.LocalParameter, "Local" },
            { LiteKitTextKey.Saved, "Saved" },
            { LiteKitTextKey.NotSaved, "Not Saved" },
            { LiteKitTextKey.Excluded, "Excluded" },
            { LiteKitTextKey.Included, "Included" },
            { LiteKitTextKey.MenuBuilderNotice, "MenuBuilder compression was detected. Combining it with Parameter Compressor is not recommended. Disable compression in MenuBuilder." },
            { LiteKitTextKey.VrcFuryNotice, "Likely VRCFury sync parameters were detected and are automatically excluded." },
            { LiteKitTextKey.FaceTrackingNotice, "Face Tracking parameters were detected and automatically excluded because they update frequently." },
            { LiteKitTextKey.ReportTitle, "# VRC Avatar LiteKit Plus Analysis Report" },
            { LiteKitTextKey.ReportMetadata, "- Generated: {0}\n- Avatar: {1}\n- Target Platform: {2}\n- Target Performance Rank: {3} or better\n- Estimated Performance Rank: {4}" },
            { LiteKitTextKey.ReportDisclaimer, "> This result is an estimate based on public VRChat Performance Rank thresholds.\n> It may not exactly match VRChat SDK Builder validation." },
            { LiteKitTextKey.ReportSummary, "## Summary" },
            { LiteKitTextKey.ReportPerformance, "## Performance Evaluations" },
            { LiteKitTextKey.ReportRenderers, "## Renderers" },
            { LiteKitTextKey.ReportMaterials, "## Materials" },
            { LiteKitTextKey.ReportTextures, "## Textures" },
            { LiteKitTextKey.ReportScanner, "## Scanner Warnings" },
            { LiteKitTextKey.ReportMobile, "## Mobile Warnings" },
            { LiteKitTextKey.ReportNotesHeader, "## Notes" },
            { LiteKitTextKey.ReportSummaryLine, "- Triangles: {0}\n- Renderer: {1}\n- Material: {2}\n- Texture: {3}\n- Estimated Texture Memory: {4}\n- Mobile Warnings: {5}" },
            { LiteKitTextKey.ReportMetric, "- {0}: {1} / target {2}: {3} / estimated {4}{5}" },
            { LiteKitTextKey.ReportRenderer, "- {0}: {1}, Material Slots {2}, Mesh {3}, Triangles {4}, Enabled {5}, Active {6}" },
            { LiteKitTextKey.ReportMaterial, "- {0}: Shader {1}, Slots {2}, Mobile Shader {3}, GPU Instancing {4}" },
            { LiteKitTextKey.ReportTexture, "- {0}: {1}x{2}, {3}, {4}, Importer {5}, Max {6}, Android {7}, iOS {8}" },
        };

        private static readonly Dictionary<string, string[]> DiagnosticTexts = new Dictionary<string, string[]>
        {
            { "MissingMaterial", new[] { "Materialスロットが未設定です。", "A Material slot is missing." } },
            { "MissingScript", new[] { "Missing Scriptがあります。", "A script is missing." } },
            { "NonMobileShader", new[] { "VRChat/Mobile以外のShaderを使用しています: {0}", "Material uses a non VRChat/Mobile shader: {0}" } },
            { "InstancingOff", new[] { "GPU InstancingはOFFです。推奨情報のみで、Materialは変更しません。", "GPU Instancing is OFF. This is advisory; the Material is not modified." } },
            { "LightMobile", new[] { "LightはMobileアバターでは削除されるか非対応です。", "Lights are removed or unsupported on mobile avatars." } },
            { "CameraMobile", new[] { "CameraはMobileアバターで非互換となる可能性があります。", "Cameras may be incompatible with mobile avatars." } },
            { "ClothMobile", new[] { "ClothはMobileアバター表示に対応していません。", "Cloth is not supported for mobile avatar display." } },
            { "AudioMobile", new[] { "AudioSourceはMobileアバターでは削除されます。", "Audio Sources are removed on mobile avatars." } },
            { "RigidbodyMobile", new[] { "RigidbodyはMobileアバターで制限されます。", "Rigidbodies are limited on mobile avatars." } },
            { "ColliderMobile", new[] { "ColliderはMobileアバターで制限されます。", "Colliders are limited on mobile avatars." } },
            { "JointMobile", new[] { "JointはMobileアバターで問題になる可能性があります。", "Joints may cause problems on mobile avatars." } },
            { "ConstraintMobile", new[] { "Unity ConstraintはConstraint上限に含まれます。", "Unity constraints count toward constraint limits." } },
            { "ParticleMobile", new[] { "Particle SystemはMobileアバターで強く制限されます。", "Particle Systems are heavily limited on mobile avatars." } },
            { "TrailMobile", new[] { "Trail RendererはMobileアバターで制限されます。", "Trail Renderers are limited on mobile avatars." } },
            { "LineMobile", new[] { "Line RendererはMobileアバターで制限されます。", "Line Renderers are limited on mobile avatars." } },
        };

        internal static string Diagnostic(LiteKitLanguage language, MobileCompatibilityWarning warning)
        {
            if (warning == null) return string.Empty;
            if (string.IsNullOrEmpty(warning.Code))
                return string.IsNullOrEmpty(warning.Path) ? warning.Message : warning.Path + ": " + warning.Message;
            return Diagnostic(language, new DiagnosticMessage
            {
                Code = warning.Code, Argument = warning.Argument, Path = warning.Path,
                Severity = warning.Severity, Category = warning.Category
            });
        }

        internal static string Diagnostic(LiteKitLanguage language, DiagnosticMessage diagnostic)
        {
            var text = DiagnosticTexts.TryGetValue(diagnostic.Code ?? "", out var pair)
                ? string.Format(pair[language == LiteKitLanguage.Japanese ? 0 : 1], diagnostic.Argument ?? "")
                : diagnostic.Code;
            return string.IsNullOrEmpty(diagnostic.Path) ? text : diagnostic.Path + ": " + text;
        }

        internal static string Severity(LiteKitLanguage language, string severity) =>
            language == LiteKitLanguage.English ? severity : severity == "Error" ? "エラー" : severity == "Warning" ? "警告" : "情報";

        private static readonly Dictionary<string, string> JapaneseLabels = new Dictionary<string, string>
        {
            { "Triangles", "ポリゴン数" },
            { "Texture Memory", "Textureメモリ" },
            { "Skinned Meshes", "Skinned Mesh数" },
            { "Basic Meshes", "Mesh Renderer数" },
            { "Material Slots", "Materialスロット数" },
            { "Animators", "Animator数" },
            { "Bones", "ボーン数" },
            { "Lights", "Light数" },
            { "Particle Systems", "Particle System数" },
            { "Total Particles Active", "最大Particle数" },
            { "Mesh Particle Active Polys", "Mesh Particleポリゴン数" },
            { "Particle Trails Enabled", "Particle Trail有効" },
            { "Particle Collision Enabled", "Particle Collision有効" },
            { "Trail Renderers", "Trail Renderer数" },
            { "Line Renderers", "Line Renderer数" },
            { "Cloths", "Cloth数" },
            { "Audio Sources", "AudioSource数" },
            { "Mesh Read/Write Disabled", "Mesh Read/Write無効" },
            { "Detected", "検出" },
            { "No disabled Read/Write meshes", "Read/Write無効Meshなし" },
            { "True", "はい" },
            { "False", "いいえ" },
            { "Allowed", "許可" },
            { "False required", "無効が必要" },
            { "Enabled", "有効" },
            { "Active", "アクティブ" },
            { "Unique Materials", "異なるMaterial数" },
            { "Unique Textures", "異なるTexture数" },
            { "Texture Type", "Texture種別" },
            { "Importer Max Size", "Importer最大サイズ" },
            { "Android Override", "Android個別設定" },
            { "iOS Override", "iOS個別設定" },
            { "Same-name Candidates", "同名候補" },
            { "Raw Report", "詳細レポート" },
            { "Type", "種類" },
            { "Path", "パス" },
            { "Mesh", "Mesh" },
            { "Mobile", "Mobile" },
            { "Shader", "Shader" },
            { "Unity Constraint", "Unity Constraint" },
        };

        internal static string Label(LiteKitLanguage language, string value) =>
            language == LiteKitLanguage.Japanese && JapaneseLabels.TryGetValue(value ?? "", out var translated) ? translated : value;

        internal static string MetricTarget(LiteKitLanguage language, MetricEvaluation metric, LiteKitPlatform platform)
        {
            var target = Label(language, metric.TargetText);
            // NonSerialized metadata survives only for live reports. For legacy/JSON reports,
            // typed numeric value and rank must match the numeric threshold contract.
            // The Read/Write condition has Key=Triangles, value=0, rank=VeryPoor;
            // this consistency check excludes it without interpreting display strings.
            var numeric = metric.UsesNumericUpperBound;
            if (!numeric && PerformanceRankThresholds.TryGetNumericThresholds(platform, metric.Key, out var limits))
            {
                var rank = PerformanceRank.VeryPoor;
                for (var i = 0; i < limits.Length; i++)
                    if (metric.NumericValue <= limits[i]) { rank = (PerformanceRank)i; break; }
                numeric = metric.EstimatedRank == rank;
            }
            return numeric ? "≤ " + target : target;
        }

        internal static string Boolean(LiteKitLanguage language, bool value) => Label(language, value ? "True" : "False");

        internal static string Get(LiteKitLanguage language, LiteKitTextKey key) =>
            (language == LiteKitLanguage.Japanese ? Japanese : English)[key];

        internal static string Select(string japanese, string english) =>
            CurrentLanguage == LiteKitLanguage.Japanese ? japanese : english;

        internal static LiteKitLanguage CurrentLanguage
        {
            get
            {
                var value = EditorPrefs.GetString(LanguagePreferenceKey, LiteKitLanguage.Japanese.ToString());
                return value == LiteKitLanguage.English.ToString() ? LiteKitLanguage.English : LiteKitLanguage.Japanese;
            }
            set => EditorPrefs.SetString(LanguagePreferenceKey, value.ToString());
        }
    }
}
