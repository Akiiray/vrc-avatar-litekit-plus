using System.Text;
using UnityEngine;
using Akiiray.VRCAvatarLiteKitPlus.Editor.Analyzer;
using Akiiray.VRCAvatarLiteKitPlus.Editor.Planner;
using Akiiray.VRCAvatarLiteKitPlus.Editor.Localization;

namespace Akiiray.VRCAvatarLiteKitPlus.Editor.Core
{
    public static class LiteKitReportFormatter
    {
        public static string ToMarkdown(LiteKitReport report, LiteKitLanguage language)
        {
            if (report == null) return string.Empty;
            return language == LiteKitLanguage.Japanese ? ToJapaneseMarkdown(report) : ToEnglishMarkdown(report);
        }

        public static string ToMarkdown(LiteKitReport report) => ToMarkdown(report, LiteKitLanguage.English);

        private static string ToJapaneseMarkdown(LiteKitReport report)
        {
            var builder = new StringBuilder();
            builder.AppendLine("# VRC Avatar LiteKit Plus 診断レポート");
            builder.AppendLine();
            builder.AppendLine($"- 生成日時: {report.GeneratedTime}");
            builder.AppendLine($"- Avatar: {report.AvatarName}");
            builder.AppendLine($"- 対象Platform: {report.TargetPlatform}");
            builder.AppendLine($"- 目標Performance Rank: {report.TargetRank}以上");
            builder.AppendLine($"- 推定Performance Rank: {report.EstimatedRank}");
            builder.AppendLine();
            builder.AppendLine("> この結果はVRChat公式Performance Rank閾値を基にした推定です。");
            builder.AppendLine("> VRChat SDK BuilderのValidation結果と完全一致しない場合があります。");
            AppendSummary(builder, report, true);
            AppendMetrics(builder, report, true);
            AppendRenderers(builder, report, true);
            AppendMaterials(builder, report, true);
            AppendTextures(builder, report, true);
            AppendWarnings(builder, report, true);
            builder.AppendLine("## 注記");
            builder.AppendLine(LiteKitLocalization.Get(LiteKitLanguage.Japanese, LiteKitTextKey.ReportNotes));
            return builder.ToString();
        }

        private static string ToEnglishMarkdown(LiteKitReport report)
        {
            var builder = new StringBuilder();
            builder.AppendLine("# VRC Avatar LiteKit Plus Analysis Report");
            builder.AppendLine();
            builder.AppendLine($"- Generated: {report.GeneratedTime}");
            builder.AppendLine($"- Avatar: {report.AvatarName}");
            builder.AppendLine($"- Target Platform: {report.TargetPlatform}");
            builder.AppendLine($"- Target Performance Rank: {report.TargetRank} or better");
            builder.AppendLine($"- Estimated Performance Rank: {report.EstimatedRank}");
            builder.AppendLine();
            builder.AppendLine("> This result is an estimate based on public VRChat Performance Rank thresholds.");
            builder.AppendLine("> It may not exactly match VRChat SDK Builder validation.");
            AppendSummary(builder, report, false);
            AppendMetrics(builder, report, false);
            AppendRenderers(builder, report, false);
            AppendMaterials(builder, report, false);
            AppendTextures(builder, report, false);
            AppendWarnings(builder, report, false);
            builder.AppendLine("## Notes");
            builder.AppendLine(LiteKitLocalization.Get(LiteKitLanguage.English, LiteKitTextKey.ReportNotes));
            return builder.ToString();
        }

        private static void AppendSummary(StringBuilder b, LiteKitReport r, bool ja)
        {
            b.AppendLine(); b.AppendLine(ja ? "## 概要" : "## Summary");
            b.AppendLine($"- Triangles: {r.Stats.TriangleCount}");
            b.AppendLine($"- Renderer: {r.Renderers.Count}"); b.AppendLine($"- Material: {r.Materials.Count}");
            b.AppendLine($"- Texture: {r.Textures.Count}");
            b.AppendLine($"- {(ja ? "推定Texture Memory" : "Estimated Texture Memory")}: {FormatBytes(r.Stats.EstimatedTextureMemoryBytes)}");
            b.AppendLine($"- {(ja ? "Mobile警告" : "Mobile Warnings")}: {r.MobileWarnings.Count}");
        }

        private static void AppendMetrics(StringBuilder b, LiteKitReport r, bool ja)
        {
            b.AppendLine(); b.AppendLine(ja ? "## Performance評価" : "## Performance Evaluations");
            foreach (var m in r.Evaluation.Metrics)
                b.AppendLine($"- {LiteKitLocalization.Label(ja ? LiteKitLanguage.Japanese : LiteKitLanguage.English, m.DisplayName)}: {LiteKitLocalization.Label(ja ? LiteKitLanguage.Japanese : LiteKitLanguage.English, m.ValueText)} / {(ja ? "目標" : "target")} {m.TargetRank}: {LiteKitLocalization.MetricTarget(ja ? LiteKitLanguage.Japanese : LiteKitLanguage.English, r.TargetPlatform, m)} / {(ja ? "推定" : "estimated")} {m.EstimatedRank}{(m.Excess > 0 ? $", +{m.Excess:0.##}" : string.Empty)}");
        }

        private static void AppendRenderers(StringBuilder b, LiteKitReport r, bool ja)
        {
            b.AppendLine(); b.AppendLine(ja ? "## Renderer一覧" : "## Renderers");
            foreach (var x in r.Renderers) b.AppendLine($"- {x.Path}: {x.RendererType}, Material Slots {x.MaterialSlotCount}, Mesh {x.MeshName}, Triangles {x.TriangleCount}, Enabled {x.Enabled}, Active {x.GameObjectActiveInHierarchy}");
        }

        private static void AppendMaterials(StringBuilder b, LiteKitReport r, bool ja)
        {
            b.AppendLine(); b.AppendLine(ja ? "## Material一覧" : "## Materials");
            foreach (var x in r.Materials) b.AppendLine($"- {x.Name}: Shader {x.ShaderName}, Slots {x.SlotCount}, Mobile Shader {x.UsesMobileShader}, GPU Instancing {x.EnableGpuInstancing}");
        }

        private static void AppendTextures(StringBuilder b, LiteKitReport r, bool ja)
        {
            b.AppendLine(); b.AppendLine(ja ? "## Texture一覧" : "## Textures");
            foreach (var x in r.Textures) b.AppendLine($"- {x.Name}: {x.Width}x{x.Height}, {x.TextureType}, {FormatBytes(x.EstimatedMemoryBytes)}, Importer {x.ImporterTextureType}, Max {x.ImporterMaxSize}, Android {x.AndroidOverride}, iOS {x.IosOverride}");
        }

        private static void AppendWarnings(StringBuilder b, LiteKitReport r, bool ja)
        {
            b.AppendLine(); b.AppendLine(ja ? "## Mobile警告" : "## Mobile Warnings");
            foreach (var x in r.MobileWarnings) b.AppendLine($"- [{LiteKitLocalization.Severity(ja ? LiteKitLanguage.Japanese : LiteKitLanguage.English, x.Severity)}] {x.Category} / {LiteKitLocalization.Diagnostic(ja ? LiteKitLanguage.Japanese : LiteKitLanguage.English, x)}");
            foreach (var x in r.ScannerWarnings)
                b.AppendLine($"- [{LiteKitLocalization.Severity(ja ? LiteKitLanguage.Japanese : LiteKitLanguage.English, x.Severity)}] {LiteKitLocalization.Diagnostic(ja ? LiteKitLanguage.Japanese : LiteKitLanguage.English, x)}");
            b.AppendLine();
        }

        public static string ToJson(LiteKitReport report)
        {
            return report == null ? string.Empty : JsonUtility.ToJson(report, true);
        }

        public static string FormatBytes(long bytes)
        {
            if (bytes >= 1024L * 1024L)
            {
                return $"{bytes / 1024.0 / 1024.0:0.0} MB";
            }

            return $"{bytes / 1024.0:0.0} KB";
        }
    }
}
