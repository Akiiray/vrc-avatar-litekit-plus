using System.Collections.Generic;
using UnityEditor;

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
            { LiteKitTextKey.Triangles, "Triangles" }, { LiteKitTextKey.RendererCount, "Renderer数" }, { LiteKitTextKey.MaterialCount, "Material数" },
            { LiteKitTextKey.TextureCount, "Texture数" }, { LiteKitTextKey.TextureMemory, "推定Texture Memory" },
            { LiteKitTextKey.ParameterSection, "Parameter / 最適化状態" }, { LiteKitTextKey.ParameterCompressor, "Parameter Compressor" },
            { LiteKitTextKey.NotAdded, "未導入" }, { LiteKitTextKey.Enabled, "有効" }, { LiteKitTextKey.Disabled, "無効" },
            { LiteKitTextKey.PerformanceDetails, "Performance詳細" }, { LiteKitTextKey.RendererDetails, "Renderer詳細" },
            { LiteKitTextKey.MaterialDetails, "Material詳細" }, { LiteKitTextKey.TextureDetails, "Texture詳細" },
            { LiteKitTextKey.MobileWarnings, "Mobile警告" }, { LiteKitTextKey.RawReport, "Raw Report" },
            { LiteKitTextKey.CopyMarkdown, "Markdownをコピー" }, { LiteKitTextKey.CopyJson, "JSONをコピー" },
            { LiteKitTextKey.PreviewDisclaimer, "プレビュー値はNDMF ParameterInfo等に基づく推定です。\nBuild時の最終Parameter構成によって結果が異なる場合があります。" },
            { LiteKitTextKey.MaterialNotice, "Mobile Shader判定は簡易判定です。\n現時点ではMaterial変換は行いません。" },
            { LiteKitTextKey.MobileNotice, "この警告は診断情報です。\nLiteKitはこの診断によってComponentの削除、無効化、変換を行いません。" },
            { LiteKitTextKey.Generated, "生成日時" }, { LiteKitTextKey.Platform, "Platform" }, { LiteKitTextKey.SkinnedMeshes, "Skinned Mesh" },
            { LiteKitTextKey.MeshRenderers, "Mesh Renderer" }, { LiteKitTextKey.MaterialSlots, "Material Slots" },
            { LiteKitTextKey.UniqueMaterials, "Unique Materials" }, { LiteKitTextKey.UniqueTextures, "Unique Textures" },
            { LiteKitTextKey.Path, "Path" }, { LiteKitTextKey.Type, "Type" }, { LiteKitTextKey.Mesh, "Mesh" },
            { LiteKitTextKey.EnabledLabel, "Enabled" }, { LiteKitTextKey.Active, "Active" }, { LiteKitTextKey.BlendShapes, "BlendShapes" },
            { LiteKitTextKey.Bones, "Bones" }, { LiteKitTextKey.Shader, "Shader" }, { LiteKitTextKey.Instancing, "GPU Instancing" },
            { LiteKitTextKey.MobileShader, "Mobile Shader" }, { LiteKitTextKey.SameNameCandidates, "同名候補" },
            { LiteKitTextKey.Resolution, "解像度" }, { LiteKitTextKey.Mipmap, "Mipmap" }, { LiteKitTextKey.TextureType, "Texture Type" },
            { LiteKitTextKey.ImporterMaxSize, "Importer Max Size" }, { LiteKitTextKey.AndroidOverride, "Android Override" },
            { LiteKitTextKey.IosOverride, "iOS Override" }, { LiteKitTextKey.Copied, "レポートをクリップボードへコピーしました。" }
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
            { LiteKitTextKey.PreviewDisclaimer, "Preview values are estimates based on the current NDMF parameter information.\nThe final result may differ during the avatar build." },
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
            { LiteKitTextKey.IosOverride, "iOS Override" }, { LiteKitTextKey.Copied, "Report copied to the clipboard." }
        };

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
