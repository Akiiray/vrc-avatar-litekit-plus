using System.Text;
using UnityEngine;
using Akiiray.VRCAvatarLiteKitPlus.Editor.Analyzer;
using Akiiray.VRCAvatarLiteKitPlus.Editor.Planner;

namespace Akiiray.VRCAvatarLiteKitPlus.Editor.Core
{
    public static class LiteKitReportFormatter
    {
        public static string ToMarkdown(LiteKitReport report)
        {
            if (report == null)
            {
                return string.Empty;
            }

            var builder = new StringBuilder();
            builder.AppendLine("# VRC Avatar LiteKit Plus Dry Run Report");
            builder.AppendLine();
            builder.AppendLine($"- Generated time: {report.GeneratedTime}");
            builder.AppendLine($"- Avatar name: {report.AvatarName}");
            builder.AppendLine($"- Target platform: {report.TargetPlatform}");
            builder.AppendLine($"- Target rank: {report.TargetRank}");
            builder.AppendLine($"- Estimated rank: {report.EstimatedRank}");
            builder.AppendLine();
            builder.AppendLine("> Note: LiteKit v0.1 is an independent estimate based on public VRChat Performance Rank thresholds. It is not SDK Builder validation.");
            builder.AppendLine();

            builder.AppendLine("## Summary");
            builder.AppendLine($"- Triangles: {report.Stats.TriangleCount}");
            builder.AppendLine($"- Renderers: {report.Renderers.Count}");
            builder.AppendLine($"- Materials: {report.Materials.Count}");
            builder.AppendLine($"- Textures: {report.Textures.Count}");
            builder.AppendLine($"- Texture memory estimate: {FormatBytes(report.Stats.EstimatedTextureMemoryBytes)}");
            builder.AppendLine($"- Mobile warnings: {report.MobileWarnings.Count}");
            builder.AppendLine();

            builder.AppendLine("## Metric Evaluations");
            foreach (var metric in report.Evaluation.Metrics)
            {
                var excess = metric.Excess > 0 ? $", excess +{metric.Excess:0.##}" : string.Empty;
                builder.AppendLine($"- {metric.DisplayName}: {metric.ValueText} / target {metric.TargetRank} <= {metric.TargetText} / estimated {metric.EstimatedRank}{excess}");
            }
            builder.AppendLine();

            builder.AppendLine("## Renderers Summary");
            foreach (var renderer in report.Renderers)
            {
                builder.AppendLine($"- {renderer.Path}: {renderer.RendererType}, slots {renderer.MaterialSlotCount}, mesh {renderer.MeshName}, triangles {renderer.TriangleCount}, enabled {renderer.Enabled}, active {renderer.GameObjectActiveInHierarchy}");
            }
            builder.AppendLine();

            builder.AppendLine("## Materials Summary");
            foreach (var material in report.Materials)
            {
                builder.AppendLine($"- {material.Name}: shader {material.ShaderName}, slots {material.SlotCount}, mobile shader {material.UsesMobileShader}, instancing {material.EnableGpuInstancing}");
            }
            builder.AppendLine();

            builder.AppendLine("## Textures Summary");
            foreach (var texture in report.Textures)
            {
                builder.AppendLine($"- {texture.Name}: {texture.Width}x{texture.Height}, {texture.TextureType}, {FormatBytes(texture.EstimatedMemoryBytes)}, importer {texture.ImporterTextureType}, max {texture.ImporterMaxSize}, Android override {texture.AndroidOverride}, iOS override {texture.IosOverride}");
            }
            builder.AppendLine();

            builder.AppendLine("## Mobile Warnings");
            foreach (var warning in report.MobileWarnings)
            {
                builder.AppendLine($"- [{warning.Severity}] {warning.Category} at {warning.Path}: {warning.Message}");
            }
            builder.AppendLine();

            builder.AppendLine("## Notes");
            builder.AppendLine(report.Notes);
            return builder.ToString();
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
