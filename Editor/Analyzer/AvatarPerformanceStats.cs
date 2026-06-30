using System;

namespace Akiiray.VRCAvatarLiteKitPlus.Editor.Analyzer
{
    [Serializable]
    public sealed class AvatarPerformanceStats
    {
        public int TriangleCount;
        public bool HasMeshReadWriteDisabled;
        public int SkinnedMeshRendererCount;
        public int MeshRendererCount;
        public int MaterialSlotCount;
        public int UniqueMaterialCount;
        public int UniqueTextureCount;
        public long EstimatedTextureMemoryBytes;
        public int AnimatorCount;
        public int TransformCount;
        public int BoneCount;
        public int LightCount;
        public int ParticleSystemCount;
        public int ParticleMaxParticles;
        public int MeshParticleTriangleEstimate;
        public bool HasParticleTrails;
        public bool HasParticleCollision;
        public int TrailRendererCount;
        public int LineRendererCount;
        public int ClothCount;
        public int AudioSourceCount;
    }
}
