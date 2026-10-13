using System;
using System.Collections.Generic;
using Akiiray.VRCAvatarLiteKitPlus.Editor.Analyzer;
using Akiiray.VRCAvatarLiteKitPlus.Editor.Planner;

namespace Akiiray.VRCAvatarLiteKitPlus.Editor.Core
{
    [Serializable]
    public sealed class LiteKitReport
    {
        [NonSerialized] public UnityEngine.GameObject AnalyzedAvatar;
        public bool AnalysisCompleted;
        public bool IsStale;

        public bool Matches(UnityEngine.GameObject avatar, LiteKitPlatform platform, PerformanceRank rank) =>
            AnalysisCompleted && !IsStale && AnalyzedAvatar != null &&
            AnalyzedAvatar == avatar && TargetPlatform == platform && TargetRank == rank;

        public string GeneratedTime;
        public string AvatarName;
        public LiteKitPlatform TargetPlatform;
        public PerformanceRank TargetRank;
        public PerformanceRank EstimatedRank;
        public AvatarPerformanceStats Stats;
        public PerformanceEvaluationResult Evaluation;
        public List<RendererUsageInfo> Renderers = new List<RendererUsageInfo>();
        public List<MaterialUsageInfo> Materials = new List<MaterialUsageInfo>();
        public List<TextureUsageInfo> Textures = new List<TextureUsageInfo>();
        public List<MobileCompatibilityWarning> MobileWarnings = new List<MobileCompatibilityWarning>();
        // Preserve the public JSON string-array contract and original English messages.
        public List<string> ScannerWarnings = new List<string>();
        public List<DiagnosticMessage> ScannerDiagnostics = new List<DiagnosticMessage>();
        public string Notes;
    }
}
