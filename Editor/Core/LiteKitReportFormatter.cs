using System;
using System.Text;
using UnityEngine;
using Akiiray.VRCAvatarLiteKitPlus.Editor.Localization;

namespace Akiiray.VRCAvatarLiteKitPlus.Editor.Core
{
    public static class LiteKitReportFormatter
    {
        public static string ToMarkdown(LiteKitReport report, LiteKitLanguage language)
        {
            if (report == null) return string.Empty;
            var b = new StringBuilder();
            Func<LiteKitTextKey, string> t = key => LiteKitLocalization.Get(language, key);
            Func<string, string> label = value => LiteKitLocalization.Label(language, value);
            b.AppendLine(t(LiteKitTextKey.ReportTitle)); b.AppendLine();
            b.AppendLine(string.Format(t(LiteKitTextKey.ReportMetadata), report.GeneratedTime, report.AvatarName,
                report.TargetPlatform, report.TargetRank, report.EstimatedRank));
            b.AppendLine(); b.AppendLine(t(LiteKitTextKey.ReportDisclaimer));
            b.AppendLine(); b.AppendLine(t(LiteKitTextKey.ReportSummary));
            b.AppendLine(string.Format(t(LiteKitTextKey.ReportSummaryLine), report.Stats?.TriangleCount ?? 0,
                report.Renderers?.Count ?? 0, report.Materials?.Count ?? 0, report.Textures?.Count ?? 0,
                FormatBytes(report.Stats?.EstimatedTextureMemoryBytes ?? 0), report.MobileWarnings?.Count ?? 0));
            b.AppendLine(); b.AppendLine(t(LiteKitTextKey.ReportPerformance));
            if (report.Evaluation?.Metrics != null)
                foreach (var m in report.Evaluation.Metrics)
                {
                    if (m == null) continue;
                    b.AppendLine(string.Format(t(LiteKitTextKey.ReportMetric), label(m.DisplayName), label(m.ValueText),
                        m.TargetRank, LiteKitLocalization.MetricTarget(language, m, report.TargetPlatform), m.EstimatedRank,
                        m.Excess > 0 ? $", +{m.Excess:0.##}" : string.Empty));
                }
            b.AppendLine(); b.AppendLine(t(LiteKitTextKey.ReportRenderers));
            if (report.Renderers != null)
                foreach (var x in report.Renderers)
                    if (x != null) b.AppendLine(string.Format(t(LiteKitTextKey.ReportRenderer), x.Path, x.RendererType,
                        x.MaterialSlotCount, x.MeshName, x.TriangleCount, LiteKitLocalization.Boolean(language, x.Enabled),
                        LiteKitLocalization.Boolean(language, x.GameObjectActiveInHierarchy)));
            b.AppendLine(); b.AppendLine(t(LiteKitTextKey.ReportMaterials));
            if (report.Materials != null)
                foreach (var x in report.Materials)
                    if (x != null) b.AppendLine(string.Format(t(LiteKitTextKey.ReportMaterial), x.Name, x.ShaderName,
                        x.SlotCount, LiteKitLocalization.Boolean(language, x.UsesMobileShader), LiteKitLocalization.Boolean(language, x.EnableGpuInstancing)));
            b.AppendLine(); b.AppendLine(t(LiteKitTextKey.ReportTextures));
            if (report.Textures != null)
                foreach (var x in report.Textures)
                    if (x != null) b.AppendLine(string.Format(t(LiteKitTextKey.ReportTexture), x.Name, x.Width, x.Height,
                        x.TextureType, FormatBytes(x.EstimatedMemoryBytes), x.ImporterTextureType, x.ImporterMaxSize,
                        LiteKitLocalization.Boolean(language, x.AndroidOverride), LiteKitLocalization.Boolean(language, x.IosOverride)));
            b.AppendLine(); b.AppendLine(t(LiteKitTextKey.ReportScanner));
            if (report.ScannerDiagnostics != null && report.ScannerDiagnostics.Count > 0)
            {
                foreach (var x in report.ScannerDiagnostics)
                    if (x != null) b.AppendLine($"- [{LiteKitLocalization.Severity(language, x.Severity)}] {LiteKitLocalization.Diagnostic(language, x)}");
            }
            else if (report.ScannerWarnings != null)
                foreach (var warning in report.ScannerWarnings) b.AppendLine("- " + warning);
            b.AppendLine(); b.AppendLine(t(LiteKitTextKey.ReportMobile));
            if (report.MobileWarnings != null)
                foreach (var x in report.MobileWarnings)
                    if (x != null) b.AppendLine($"- [{LiteKitLocalization.Severity(language, x.Severity)}] {x.Category} / {LiteKitLocalization.Diagnostic(language, x)}");
            b.AppendLine(); b.AppendLine(t(LiteKitTextKey.ReportNotesHeader));
            b.AppendLine(string.IsNullOrEmpty(report.Notes) ? t(LiteKitTextKey.ReportNotes) : report.Notes);
            return b.ToString();
        }

        public static string ToMarkdown(LiteKitReport report) => ToMarkdown(report, LiteKitLanguage.English);

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
