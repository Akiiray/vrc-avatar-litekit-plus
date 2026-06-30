using System;
using System.Collections.Generic;
using Akiiray.VRCAvatarLiteKitPlus.Editor.Analyzer;

namespace Akiiray.VRCAvatarLiteKitPlus.Editor.Planner
{
    [Serializable]
    public sealed class MetricEvaluation
    {
        public PerformanceMetricKey Key;
        public string DisplayName;
        public string ValueText;
        public double NumericValue;
        public PerformanceRank EstimatedRank;
        public PerformanceRank TargetRank;
        public string TargetText;
        public double Excess;
        public bool BlocksTarget;
        public string Note;
    }

    [Serializable]
    public sealed class TargetRankPlan
    {
        public PerformanceRank TargetRank;
        public List<MetricEvaluation> BlockingMetrics = new List<MetricEvaluation>();
    }

    [Serializable]
    public sealed class PerformanceEvaluationResult
    {
        public LiteKitPlatform Platform;
        public PerformanceRank EstimatedRank;
        public PerformanceRank TargetRank;
        public List<MetricEvaluation> Metrics = new List<MetricEvaluation>();
        public TargetRankPlan TargetPlan = new TargetRankPlan();
    }

    public sealed class PerformanceRankEvaluator
    {
        public PerformanceEvaluationResult Evaluate(AvatarPerformanceStats stats, LiteKitPlatform platform, PerformanceRank targetRank)
        {
            var result = new PerformanceEvaluationResult
            {
                Platform = platform,
                EstimatedRank = PerformanceRank.Excellent,
                TargetRank = targetRank,
                TargetPlan = { TargetRank = targetRank }
            };

            AddNumeric(result, platform, targetRank, PerformanceMetricKey.Triangles, "Triangles", stats.TriangleCount, "");
            AddNumeric(result, platform, targetRank, PerformanceMetricKey.TextureMemory, "Texture Memory", BytesToMegabytes(stats.EstimatedTextureMemoryBytes), " MB");
            AddNumeric(result, platform, targetRank, PerformanceMetricKey.SkinnedMeshCount, "Skinned Meshes", stats.SkinnedMeshRendererCount, "");
            AddNumeric(result, platform, targetRank, PerformanceMetricKey.MeshRendererCount, "Basic Meshes", stats.MeshRendererCount, "");
            AddNumeric(result, platform, targetRank, PerformanceMetricKey.MaterialSlotCount, "Material Slots", stats.MaterialSlotCount, "");
            AddNumeric(result, platform, targetRank, PerformanceMetricKey.AnimatorCount, "Animators", stats.AnimatorCount, "");
            AddNumeric(result, platform, targetRank, PerformanceMetricKey.BoneCount, "Bones", stats.BoneCount, "");
            AddNumeric(result, platform, targetRank, PerformanceMetricKey.LightCount, "Lights", stats.LightCount, "");
            AddNumeric(result, platform, targetRank, PerformanceMetricKey.ParticleSystemCount, "Particle Systems", stats.ParticleSystemCount, "");
            AddNumeric(result, platform, targetRank, PerformanceMetricKey.ParticleCount, "Total Particles Active", stats.ParticleMaxParticles, "");
            AddNumeric(result, platform, targetRank, PerformanceMetricKey.MeshParticleActivePolys, "Mesh Particle Active Polys", stats.MeshParticleTriangleEstimate, "");
            AddBoolean(result, platform, targetRank, PerformanceMetricKey.ParticleTrailsEnabled, "Particle Trails Enabled", stats.HasParticleTrails);
            AddBoolean(result, platform, targetRank, PerformanceMetricKey.ParticleCollisionEnabled, "Particle Collision Enabled", stats.HasParticleCollision);
            AddNumeric(result, platform, targetRank, PerformanceMetricKey.TrailRendererCount, "Trail Renderers", stats.TrailRendererCount, "");
            AddNumeric(result, platform, targetRank, PerformanceMetricKey.LineRendererCount, "Line Renderers", stats.LineRendererCount, "");
            AddNumeric(result, platform, targetRank, PerformanceMetricKey.ClothCount, "Cloths", stats.ClothCount, "");
            AddNumeric(result, platform, targetRank, PerformanceMetricKey.AudioSourceCount, "Audio Sources", stats.AudioSourceCount, "");

            if (stats.HasMeshReadWriteDisabled)
            {
                var metric = new MetricEvaluation
                {
                    Key = PerformanceMetricKey.Triangles,
                    DisplayName = "Mesh Read/Write Disabled",
                    ValueText = "Detected",
                    EstimatedRank = PerformanceRank.VeryPoor,
                    TargetRank = targetRank,
                    TargetText = "No disabled Read/Write meshes",
                    BlocksTarget = true,
                    Note = "VRChat states Mesh Read/Write Disabled downgrades the Performance Rank to Very Poor. LiteKit does not modify meshes."
                };
                result.Metrics.Add(metric);
                result.TargetPlan.BlockingMetrics.Add(metric);
                result.EstimatedRank = PerformanceRank.VeryPoor;
            }

            return result;
        }

        private static void AddNumeric(PerformanceEvaluationResult result, LiteKitPlatform platform, PerformanceRank targetRank, PerformanceMetricKey key, string displayName, double value, string unit)
        {
            if (!PerformanceRankThresholds.TryGetNumericThresholds(platform, key, out var thresholds))
            {
                return;
            }

            var rank = RankNumeric(value, thresholds);
            var targetThreshold = thresholds[(int)targetRank];
            var excess = Math.Max(0, value - targetThreshold);
            var metric = new MetricEvaluation
            {
                Key = key,
                DisplayName = displayName,
                NumericValue = value,
                ValueText = FormatNumber(value) + unit,
                EstimatedRank = rank,
                TargetRank = targetRank,
                TargetText = FormatNumber(targetThreshold) + unit,
                Excess = excess,
                BlocksTarget = rank > targetRank,
                Note = "LiteKit v0.1 estimate based on VRChat public thresholds, not SDK Builder validation."
            };
            result.Metrics.Add(metric);
            if (metric.BlocksTarget)
            {
                result.TargetPlan.BlockingMetrics.Add(metric);
            }

            if (rank > result.EstimatedRank)
            {
                result.EstimatedRank = rank;
            }
        }

        private static void AddBoolean(PerformanceEvaluationResult result, LiteKitPlatform platform, PerformanceRank targetRank, PerformanceMetricKey key, string displayName, bool value)
        {
            if (!PerformanceRankThresholds.TryGetBooleanThresholds(platform, key, out var thresholds))
            {
                return;
            }

            var rank = RankBoolean(value, thresholds);
            var targetAllowed = thresholds[(int)targetRank];
            var metric = new MetricEvaluation
            {
                Key = key,
                DisplayName = displayName,
                ValueText = value ? "True" : "False",
                NumericValue = value ? 1 : 0,
                EstimatedRank = rank,
                TargetRank = targetRank,
                TargetText = targetAllowed ? "Allowed" : "False required",
                Excess = value && !targetAllowed ? 1 : 0,
                BlocksTarget = rank > targetRank,
                Note = "LiteKit v0.1 estimate based on VRChat public thresholds, not SDK Builder validation."
            };
            result.Metrics.Add(metric);
            if (metric.BlocksTarget)
            {
                result.TargetPlan.BlockingMetrics.Add(metric);
            }

            if (rank > result.EstimatedRank)
            {
                result.EstimatedRank = rank;
            }
        }

        private static PerformanceRank RankNumeric(double value, int[] thresholds)
        {
            if (value <= thresholds[0]) return PerformanceRank.Excellent;
            if (value <= thresholds[1]) return PerformanceRank.Good;
            if (value <= thresholds[2]) return PerformanceRank.Medium;
            if (value <= thresholds[3]) return PerformanceRank.Poor;
            return PerformanceRank.VeryPoor;
        }

        private static PerformanceRank RankBoolean(bool value, bool[] thresholds)
        {
            for (var i = 0; i < thresholds.Length; i++)
            {
                if (value == thresholds[i])
                {
                    return (PerformanceRank)i;
                }
            }

            return PerformanceRank.VeryPoor;
        }

        private static double BytesToMegabytes(long bytes)
        {
            return bytes / 1024.0 / 1024.0;
        }

        private static string FormatNumber(double value)
        {
            return Math.Abs(value % 1) < 0.01 ? ((int)value).ToString() : value.ToString("0.0");
        }
    }
}
