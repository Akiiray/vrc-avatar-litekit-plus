using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Akiiray.VRCAvatarLiteKitPlus.Editor.Core;
using Akiiray.VRCAvatarLiteKitPlus.Editor.Utilities;

namespace Akiiray.VRCAvatarLiteKitPlus.Editor.Analyzer
{
    public static class AvatarScanner
    {
        public static AvatarContext Scan(GameObject avatarRoot)
        {
            if (avatarRoot == null)
            {
                LiteKitLogger.Warning("Avatar Root is not selected. Select an avatar root or assign one in the LiteKit window.");
                return null;
            }

            var warnings = new List<string>();
            var renderers = GetComponents<Renderer>(avatarRoot);
            var materials = new HashSet<Material>();
            var textures = new HashSet<Texture>();

            foreach (var renderer in renderers)
            {
                foreach (var material in renderer.sharedMaterials)
                {
                    if (material == null)
                    {
                        warnings.Add($"Renderer '{GetPath(renderer.transform)}' has a missing Material slot.");
                        continue;
                    }

                    materials.Add(material);
                    CollectMaterialTextures(material, textures);
                }
            }

            foreach (var transform in GetComponents<Transform>(avatarRoot))
            {
                var components = transform.GetComponents<Component>();
                for (var i = 0; i < components.Length; i++)
                {
                    if (components[i] == null)
                    {
                        warnings.Add($"Missing script on '{GetPath(transform)}'.");
                    }
                }
            }

            foreach (var warning in warnings)
            {
                LiteKitLogger.Warning(warning);
            }

            return new AvatarContext(
                avatarRoot,
                GetComponents<Transform>(avatarRoot),
                renderers,
                GetComponents<SkinnedMeshRenderer>(avatarRoot),
                GetComponents<MeshRenderer>(avatarRoot),
                GetComponents<MeshFilter>(avatarRoot),
                new List<Material>(materials),
                new List<Texture>(textures),
                GetComponents<Animator>(avatarRoot),
                GetComponents<ParticleSystem>(avatarRoot),
                GetComponents<Light>(avatarRoot),
                GetComponents<AudioSource>(avatarRoot),
                GetComponents<TrailRenderer>(avatarRoot),
                GetComponents<LineRenderer>(avatarRoot),
                GetComponents<Cloth>(avatarRoot),
                warnings);
        }

        public static string GetPath(Transform transform)
        {
            if (transform == null)
            {
                return "<missing>";
            }

            var names = new Stack<string>();
            var current = transform;
            while (current != null)
            {
                names.Push(current.name);
                current = current.parent;
            }

            return string.Join("/", names.ToArray());
        }

        private static List<T> GetComponents<T>(GameObject root) where T : Component
        {
            return new List<T>(root.GetComponentsInChildren<T>(true));
        }

        private static void CollectMaterialTextures(Material material, HashSet<Texture> textures)
        {
            if (material == null || material.shader == null)
            {
                return;
            }

            var shader = material.shader;
            var propertyCount = ShaderUtil.GetPropertyCount(shader);
            for (var i = 0; i < propertyCount; i++)
            {
                if (ShaderUtil.GetPropertyType(shader, i) != ShaderUtil.ShaderPropertyType.TexEnv)
                {
                    continue;
                }

                var propertyName = ShaderUtil.GetPropertyName(shader, i);
                var texture = material.GetTexture(propertyName);
                if (texture != null)
                {
                    textures.Add(texture);
                }
            }
        }
    }
}
