using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Akiiray.VRCAvatarLiteKitPlus.Editor.Core;

namespace Akiiray.VRCAvatarLiteKitPlus.Editor.Analyzer
{
    [Serializable]
    public sealed class TextureUsageInfo
    {
        public string Name;
        public string TextureType;
        public int Width;
        public int Height;
        public bool HasMipMap;
        public string ImporterTextureType;
        public int ImporterMaxSize;
        public bool AndroidOverride;
        public bool IosOverride;
        public long EstimatedMemoryBytes;
        public List<string> MaterialReferences = new List<string>();
        public List<string> ShaderProperties = new List<string>();
    }

    public sealed class TextureUsageAnalyzer
    {
        public List<TextureUsageInfo> Analyze(AvatarContext context)
        {
            var results = new List<TextureUsageInfo>();
            if (context == null || !context.IsValid)
            {
                return results;
            }

            var byTexture = new Dictionary<Texture, TextureUsageInfo>();
            foreach (var material in context.Materials)
            {
                CollectMaterialTextures(material, byTexture);
            }

            results.AddRange(byTexture.Values);
            results.Sort((a, b) => string.Compare(a.Name, b.Name, StringComparison.OrdinalIgnoreCase));
            return results;
        }

        public void AnalyzeDryRun(AvatarContext context)
        {
            Analyze(context);
        }

        public static long EstimateTextureMemoryBytes(Texture texture)
        {
            if (texture == null)
            {
                return 0;
            }

            var bytesPerPixel = texture is Texture2D ? 4L : 4L;
            var baseBytes = (long)Mathf.Max(1, texture.width) * Mathf.Max(1, texture.height) * bytesPerPixel;
            return TextureHasMipMap(texture) ? (long)(baseBytes * 1.3333333f) : baseBytes;
        }

        private static void CollectMaterialTextures(Material material, Dictionary<Texture, TextureUsageInfo> byTexture)
        {
            if (material == null || material.shader == null)
            {
                return;
            }

            var shader = material.shader;
            for (var i = 0; i < ShaderUtil.GetPropertyCount(shader); i++)
            {
                if (ShaderUtil.GetPropertyType(shader, i) != ShaderUtil.ShaderPropertyType.TexEnv)
                {
                    continue;
                }

                var propertyName = ShaderUtil.GetPropertyName(shader, i);
                var texture = material.GetTexture(propertyName);
                if (texture == null)
                {
                    continue;
                }

                if (!byTexture.TryGetValue(texture, out var info))
                {
                    info = CreateTextureInfo(texture);
                    byTexture.Add(texture, info);
                }

                AddUnique(info.MaterialReferences, material.name);
                AddUnique(info.ShaderProperties, propertyName);
            }
        }

        private static TextureUsageInfo CreateTextureInfo(Texture texture)
        {
            var info = new TextureUsageInfo
            {
                Name = texture.name,
                TextureType = texture.GetType().Name,
                Width = texture.width,
                Height = texture.height,
                HasMipMap = TextureHasMipMap(texture),
                EstimatedMemoryBytes = EstimateTextureMemoryBytes(texture)
            };

            var path = AssetDatabase.GetAssetPath(texture);
            var importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer != null)
            {
                info.ImporterTextureType = importer.textureType.ToString();
                info.ImporterMaxSize = importer.maxTextureSize;
                info.AndroidOverride = importer.GetPlatformTextureSettings("Android").overridden;
                info.IosOverride = importer.GetPlatformTextureSettings("iPhone").overridden;
            }
            else
            {
                info.ImporterTextureType = "<none>";
                info.ImporterMaxSize = 0;
            }

            return info;
        }

        private static bool TextureHasMipMap(Texture texture)
        {
            if (texture is Texture2D texture2D)
            {
                return texture2D.mipmapCount > 1;
            }

            return false;
        }

        private static void AddUnique(List<string> list, string value)
        {
            if (!string.IsNullOrEmpty(value) && !list.Contains(value))
            {
                list.Add(value);
            }
        }
    }
}
