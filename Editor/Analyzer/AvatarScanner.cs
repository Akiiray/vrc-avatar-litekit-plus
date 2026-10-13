using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using Akiiray.VRCAvatarLiteKitPlus.Editor.Localization;
using UnityEngine;
using UnityTexture = UnityEngine.Texture;
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
                LiteKitLogger.Warning(LiteKitLocalization.Get(LiteKitLocalization.CurrentLanguage, LiteKitTextKey.NoAvatarSelected));
                return null;
            }

            var warnings = new List<DiagnosticMessage>();
            var renderers = GetComponents<Renderer>(avatarRoot);
            var materials = new HashSet<Material>();
            var textures = new HashSet<UnityTexture>();

            foreach (var renderer in renderers)
            {
                foreach (var material in renderer.sharedMaterials)
                {
                    if (material == null)
                    {
                        warnings.Add(new DiagnosticMessage { Code = "MissingMaterial", Path = GetPath(renderer.transform), Severity = "Warning" });
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
                        warnings.Add(new DiagnosticMessage { Code = "MissingScript", Path = GetPath(transform), Severity = "Warning" });
                    }
                }
            }

            foreach (var warning in warnings)
            {
                LiteKitLogger.Warning(LiteKitLocalization.Diagnostic(LiteKitLocalization.CurrentLanguage, warning));
            }

            return new AvatarContext(
                avatarRoot,
                GetComponents<Transform>(avatarRoot),
                renderers,
                GetComponents<SkinnedMeshRenderer>(avatarRoot),
                GetComponents<MeshRenderer>(avatarRoot),
                GetComponents<MeshFilter>(avatarRoot),
                new List<Material>(materials),
                new List<UnityTexture>(textures),
                GetComponents<Animator>(avatarRoot),
                GetComponents<ParticleSystem>(avatarRoot),
                GetComponents<Light>(avatarRoot),
                GetComponents<AudioSource>(avatarRoot),
                GetComponents<TrailRenderer>(avatarRoot),
                GetComponents<LineRenderer>(avatarRoot),
                GetComponents<Cloth>(avatarRoot),
                warnings.Select(LegacyWarning).ToList(),
                warnings);
        }

        // Legacy JSON projection is separate from localized UI presentation.
        internal static string LegacyWarning(DiagnosticMessage diagnostic)
        {
            switch (diagnostic.Code)
            {
                case "MissingMaterial": return $"Renderer '{diagnostic.Path}' has a missing Material slot.";
                case "MissingScript": return $"Missing script on '{diagnostic.Path}'.";
                default: return diagnostic.Code;
            }
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

        private static void CollectMaterialTextures(Material material, HashSet<UnityTexture> textures)
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
