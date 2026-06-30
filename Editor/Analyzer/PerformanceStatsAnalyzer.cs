using UnityEngine;
using Akiiray.VRCAvatarLiteKitPlus.Editor.Core;

namespace Akiiray.VRCAvatarLiteKitPlus.Editor.Analyzer
{
    public sealed class PerformanceStatsAnalyzer
    {
        public AvatarPerformanceStats Analyze(AvatarContext context)
        {
            var stats = new AvatarPerformanceStats();
            if (context == null || !context.IsValid)
            {
                return stats;
            }

            stats.SkinnedMeshRendererCount = context.SkinnedMeshRenderers.Count;
            stats.MeshRendererCount = context.MeshRenderers.Count;
            stats.UniqueMaterialCount = context.Materials.Count;
            stats.UniqueTextureCount = context.Textures.Count;
            stats.AnimatorCount = context.Animators.Count;
            stats.TransformCount = context.AllTransforms.Count;
            stats.BoneCount = EstimateBoneCount(context);
            stats.LightCount = context.Lights.Count;
            stats.ParticleSystemCount = context.ParticleSystems.Count;
            stats.TrailRendererCount = context.TrailRenderers.Count;
            stats.LineRendererCount = context.LineRenderers.Count;
            stats.ClothCount = context.Cloths.Count;
            stats.AudioSourceCount = context.AudioSources.Count;

            foreach (var renderer in context.Renderers)
            {
                stats.MaterialSlotCount += renderer.sharedMaterials.Length;
            }

            foreach (var skinned in context.SkinnedMeshRenderers)
            {
                AddMeshStats(skinned.sharedMesh, stats);
            }

            foreach (var meshFilter in context.MeshFilters)
            {
                AddMeshStats(meshFilter.sharedMesh, stats);
            }

            foreach (var texture in context.Textures)
            {
                stats.EstimatedTextureMemoryBytes += TextureUsageAnalyzer.EstimateTextureMemoryBytes(texture);
            }

            foreach (var particleSystem in context.ParticleSystems)
            {
                var main = particleSystem.main;
                stats.ParticleMaxParticles += main.maxParticles;

                var trails = particleSystem.trails;
                if (trails.enabled)
                {
                    stats.HasParticleTrails = true;
                }

                var collision = particleSystem.collision;
                if (collision.enabled)
                {
                    stats.HasParticleCollision = true;
                }

                var particleRenderer = particleSystem.GetComponent<ParticleSystemRenderer>();
                if (particleRenderer != null && particleRenderer.renderMode == ParticleSystemRenderMode.Mesh && particleRenderer.mesh != null)
                {
                    stats.MeshParticleTriangleEstimate += CountTriangles(particleRenderer.mesh) * main.maxParticles;
                    if (!particleRenderer.mesh.isReadable)
                    {
                        stats.HasMeshReadWriteDisabled = true;
                    }
                }
            }

            // TODO(v0.2+): Add SDK-backed PhysBone, Contact, VRCRaycast, and VRC Constraint counts
            // after introducing explicit VRChat SDK dependency and avoiding SDK Build API calls.
            return stats;
        }

        private static int EstimateBoneCount(AvatarContext context)
        {
            var bones = 0;
            foreach (var skinned in context.SkinnedMeshRenderers)
            {
                bones += skinned.bones != null ? skinned.bones.Length : 0;
            }

            return bones > 0 ? bones : context.AllTransforms.Count;
        }

        private static void AddMeshStats(Mesh mesh, AvatarPerformanceStats stats)
        {
            if (mesh == null)
            {
                return;
            }

            stats.TriangleCount += CountTriangles(mesh);
            if (!mesh.isReadable)
            {
                stats.HasMeshReadWriteDisabled = true;
            }
        }

        private static int CountTriangles(Mesh mesh)
        {
            if (mesh == null)
            {
                return 0;
            }

            var triangles = 0;
            for (var i = 0; i < mesh.subMeshCount; i++)
            {
                triangles += (int)(mesh.GetIndexCount(i) / 3);
            }

            return triangles;
        }
    }
}
