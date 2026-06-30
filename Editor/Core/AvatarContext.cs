using System.Collections.Generic;
using UnityEngine;

namespace Akiiray.VRCAvatarLiteKitPlus.Editor.Core
{
    /// <summary>
    /// Non-destructive analysis context for a selected avatar root.
    /// All collections are gathered with inactive GameObjects included because VRChat's
    /// Performance Rank counts disabled GameObjects and disabled Components.
    /// </summary>
    public sealed class AvatarContext
    {
        public AvatarContext(
            GameObject avatarRoot,
            IReadOnlyList<Transform> allTransforms,
            IReadOnlyList<Renderer> renderers,
            IReadOnlyList<SkinnedMeshRenderer> skinnedMeshRenderers,
            IReadOnlyList<MeshRenderer> meshRenderers,
            IReadOnlyList<MeshFilter> meshFilters,
            IReadOnlyList<Material> materials,
            IReadOnlyList<Texture> textures,
            IReadOnlyList<Animator> animators,
            IReadOnlyList<ParticleSystem> particleSystems,
            IReadOnlyList<Light> lights,
            IReadOnlyList<AudioSource> audioSources,
            IReadOnlyList<TrailRenderer> trailRenderers,
            IReadOnlyList<LineRenderer> lineRenderers,
            IReadOnlyList<Cloth> cloths,
            IReadOnlyList<string> warnings)
        {
            AvatarRoot = avatarRoot;
            AllTransforms = allTransforms;
            Renderers = renderers;
            SkinnedMeshRenderers = skinnedMeshRenderers;
            MeshRenderers = meshRenderers;
            MeshFilters = meshFilters;
            Materials = materials;
            Textures = textures;
            Animators = animators;
            ParticleSystems = particleSystems;
            Lights = lights;
            AudioSources = audioSources;
            TrailRenderers = trailRenderers;
            LineRenderers = lineRenderers;
            Cloths = cloths;
            Warnings = warnings;
        }

        public GameObject AvatarRoot { get; }
        public IReadOnlyList<Transform> AllTransforms { get; }
        public IReadOnlyList<Renderer> Renderers { get; }
        public IReadOnlyList<SkinnedMeshRenderer> SkinnedMeshRenderers { get; }
        public IReadOnlyList<MeshRenderer> MeshRenderers { get; }
        public IReadOnlyList<MeshFilter> MeshFilters { get; }
        public IReadOnlyList<Material> Materials { get; }
        public IReadOnlyList<Texture> Textures { get; }
        public IReadOnlyList<Animator> Animators { get; }
        public IReadOnlyList<ParticleSystem> ParticleSystems { get; }
        public IReadOnlyList<Light> Lights { get; }
        public IReadOnlyList<AudioSource> AudioSources { get; }
        public IReadOnlyList<TrailRenderer> TrailRenderers { get; }
        public IReadOnlyList<LineRenderer> LineRenderers { get; }
        public IReadOnlyList<Cloth> Cloths { get; }
        public IReadOnlyList<string> Warnings { get; }
        public bool IsValid => AvatarRoot != null;
    }
}
