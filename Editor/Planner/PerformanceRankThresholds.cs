using System;
using System.Collections.Generic;

namespace Akiiray.VRCAvatarLiteKitPlus.Editor.Planner
{
    public enum LiteKitPlatform
    {
        PC,
        Mobile
    }

    public enum PerformanceRank
    {
        Excellent = 0,
        Good = 1,
        Medium = 2,
        Poor = 3,
        VeryPoor = 4
    }

    public enum PerformanceMetricKey
    {
        Triangles,
        TextureMemory,
        SkinnedMeshCount,
        MeshRendererCount,
        MaterialSlotCount,
        PhysBoneComponentCount,
        PhysBoneAffectedTransformCount,
        PhysBoneColliderCount,
        PhysBoneCollisionCheckCount,
        ContactCount,
        ConstraintCount,
        ConstraintDepth,
        AnimatorCount,
        BoneCount,
        LightCount,
        ParticleSystemCount,
        ParticleCount,
        MeshParticleActivePolys,
        ParticleTrailsEnabled,
        ParticleCollisionEnabled,
        TrailRendererCount,
        LineRendererCount,
        RaycastCount,
        ClothCount,
        ClothVertexCount,
        PhysicsColliderCount,
        PhysicsRigidbodyCount,
        AudioSourceCount
    }

    public static class PerformanceRankThresholds
    {
        public const string SourceUrl = "https://creators.vrchat.com/avatars/avatar-performance-ranking-system/";

        private static readonly Dictionary<LiteKitPlatform, Dictionary<PerformanceMetricKey, int[]>> NumericThresholds =
            new Dictionary<LiteKitPlatform, Dictionary<PerformanceMetricKey, int[]>>
            {
                // Thresholds copied from VRChat Performance Ranks, Value Maximums per Rank.
                // Source: https://creators.vrchat.com/avatars/avatar-performance-ranking-system/
                [LiteKitPlatform.PC] = new Dictionary<PerformanceMetricKey, int[]>
                {
                    [PerformanceMetricKey.Triangles] = Values(32000, 70000, 70000, 70000),
                    [PerformanceMetricKey.TextureMemory] = Values(40, 75, 110, 150),
                    [PerformanceMetricKey.SkinnedMeshCount] = Values(1, 2, 8, 16),
                    [PerformanceMetricKey.MeshRendererCount] = Values(4, 8, 16, 24),
                    [PerformanceMetricKey.MaterialSlotCount] = Values(4, 8, 16, 32),
                    [PerformanceMetricKey.PhysBoneComponentCount] = Values(4, 8, 16, 32),
                    [PerformanceMetricKey.PhysBoneAffectedTransformCount] = Values(16, 64, 128, 256),
                    [PerformanceMetricKey.PhysBoneColliderCount] = Values(4, 8, 16, 32),
                    [PerformanceMetricKey.PhysBoneCollisionCheckCount] = Values(32, 128, 256, 512),
                    [PerformanceMetricKey.ContactCount] = Values(8, 16, 24, 32),
                    [PerformanceMetricKey.ConstraintCount] = Values(100, 250, 300, 350),
                    [PerformanceMetricKey.ConstraintDepth] = Values(20, 50, 80, 100),
                    [PerformanceMetricKey.AnimatorCount] = Values(1, 4, 16, 32),
                    [PerformanceMetricKey.BoneCount] = Values(75, 150, 256, 400),
                    [PerformanceMetricKey.LightCount] = Values(0, 0, 0, 1),
                    [PerformanceMetricKey.ParticleSystemCount] = Values(0, 4, 8, 16),
                    [PerformanceMetricKey.ParticleCount] = Values(0, 300, 1000, 2500),
                    [PerformanceMetricKey.MeshParticleActivePolys] = Values(0, 1000, 2000, 5000),
                    [PerformanceMetricKey.TrailRendererCount] = Values(1, 2, 4, 8),
                    [PerformanceMetricKey.LineRendererCount] = Values(1, 2, 4, 8),
                    [PerformanceMetricKey.RaycastCount] = Values(1, 4, 8, 15),
                    [PerformanceMetricKey.ClothCount] = Values(0, 1, 1, 1),
                    [PerformanceMetricKey.ClothVertexCount] = Values(0, 50, 100, 200),
                    [PerformanceMetricKey.PhysicsColliderCount] = Values(0, 1, 8, 8),
                    [PerformanceMetricKey.PhysicsRigidbodyCount] = Values(0, 1, 8, 8),
                    [PerformanceMetricKey.AudioSourceCount] = Values(1, 4, 8, 8)
                },
                [LiteKitPlatform.Mobile] = new Dictionary<PerformanceMetricKey, int[]>
                {
                    [PerformanceMetricKey.Triangles] = Values(7500, 10000, 15000, 20000),
                    [PerformanceMetricKey.TextureMemory] = Values(10, 18, 25, 40),
                    [PerformanceMetricKey.SkinnedMeshCount] = Values(1, 1, 2, 2),
                    [PerformanceMetricKey.MeshRendererCount] = Values(1, 1, 2, 2),
                    [PerformanceMetricKey.MaterialSlotCount] = Values(1, 1, 2, 4),
                    [PerformanceMetricKey.AnimatorCount] = Values(1, 1, 1, 2),
                    [PerformanceMetricKey.BoneCount] = Values(75, 90, 150, 150),
                    [PerformanceMetricKey.PhysBoneComponentCount] = Values(0, 4, 6, 8),
                    [PerformanceMetricKey.PhysBoneAffectedTransformCount] = Values(0, 16, 32, 64),
                    [PerformanceMetricKey.PhysBoneColliderCount] = Values(0, 4, 8, 16),
                    [PerformanceMetricKey.PhysBoneCollisionCheckCount] = Values(0, 16, 32, 64),
                    [PerformanceMetricKey.ContactCount] = Values(2, 4, 8, 16),
                    [PerformanceMetricKey.ConstraintCount] = Values(30, 60, 120, 150),
                    [PerformanceMetricKey.ConstraintDepth] = Values(5, 15, 35, 50),
                    [PerformanceMetricKey.ParticleSystemCount] = Values(0, 0, 0, 2),
                    [PerformanceMetricKey.ParticleCount] = Values(0, 0, 0, 200),
                    [PerformanceMetricKey.MeshParticleActivePolys] = Values(0, 0, 0, 400),
                    [PerformanceMetricKey.TrailRendererCount] = Values(0, 0, 0, 1),
                    [PerformanceMetricKey.LineRendererCount] = Values(0, 0, 0, 1),
                    [PerformanceMetricKey.RaycastCount] = Values(1, 2, 4, 8),
                    [PerformanceMetricKey.LightCount] = Values(0, 0, 0, 0),
                    [PerformanceMetricKey.ClothCount] = Values(0, 0, 0, 0),
                    [PerformanceMetricKey.AudioSourceCount] = Values(0, 0, 0, 0)
                }
            };

        private static readonly Dictionary<LiteKitPlatform, Dictionary<PerformanceMetricKey, bool[]>> BooleanThresholds =
            new Dictionary<LiteKitPlatform, Dictionary<PerformanceMetricKey, bool[]>>
            {
                [LiteKitPlatform.PC] = new Dictionary<PerformanceMetricKey, bool[]>
                {
                    [PerformanceMetricKey.ParticleTrailsEnabled] = BoolValues(false, false, true, true),
                    [PerformanceMetricKey.ParticleCollisionEnabled] = BoolValues(false, false, true, true)
                },
                [LiteKitPlatform.Mobile] = new Dictionary<PerformanceMetricKey, bool[]>
                {
                    [PerformanceMetricKey.ParticleTrailsEnabled] = BoolValues(false, false, false, true),
                    [PerformanceMetricKey.ParticleCollisionEnabled] = BoolValues(false, false, false, true)
                }
            };

        public static bool TryGetNumericThresholds(LiteKitPlatform platform, PerformanceMetricKey key, out int[] thresholds)
        {
            thresholds = null;
            return NumericThresholds.TryGetValue(platform, out var table) && table.TryGetValue(key, out thresholds);
        }

        public static bool TryGetBooleanThresholds(LiteKitPlatform platform, PerformanceMetricKey key, out bool[] thresholds)
        {
            thresholds = null;
            return BooleanThresholds.TryGetValue(platform, out var table) && table.TryGetValue(key, out thresholds);
        }

        public static int GetNumericThreshold(LiteKitPlatform platform, PerformanceMetricKey key, PerformanceRank rank)
        {
            return TryGetNumericThresholds(platform, key, out var thresholds) ? thresholds[(int)rank] : 0;
        }

        private static int[] Values(int excellent, int good, int medium, int poor)
        {
            return new[] { excellent, good, medium, poor };
        }

        private static bool[] BoolValues(bool excellent, bool good, bool medium, bool poor)
        {
            return new[] { excellent, good, medium, poor };
        }
    }
}
