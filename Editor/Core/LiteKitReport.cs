using System;
using System.Collections.Generic;
using Akiiray.VRCAvatarLiteKitPlus.Editor.Analyzer;
using Akiiray.VRCAvatarLiteKitPlus.Editor.Planner;

namespace Akiiray.VRCAvatarLiteKitPlus.Editor.Core
{
    [Serializable]
    public sealed class LiteKitReport
    {
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
        public List<string> ScannerWarnings = new List<string>();
        public string Notes;
    }
}
