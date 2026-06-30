using System;
using System.Collections.Generic;
using UnityEngine;
using Akiiray.VRCAvatarLiteKitPlus.Editor.Core;

namespace Akiiray.VRCAvatarLiteKitPlus.Editor.Analyzer
{
    [Serializable]
    public sealed class MaterialUsageInfo
    {
        public string Name;
        public string ShaderName;
        public int SlotCount;
        public bool EnableGpuInstancing;
        public bool UsesMobileShader;
        public bool HasSameNameCandidates;
        public List<string> RendererPaths = new List<string>();
    }

    public sealed class MaterialUsageAnalyzer
    {
        public List<MaterialUsageInfo> Analyze(AvatarContext context)
        {
            var results = new List<MaterialUsageInfo>();
            if (context == null || !context.IsValid)
            {
                return results;
            }

            var byMaterial = new Dictionary<Material, MaterialUsageInfo>();
            var nameCounts = new Dictionary<string, int>();

            foreach (var material in context.Materials)
            {
                if (material == null)
                {
                    continue;
                }

                nameCounts.TryGetValue(material.name, out var count);
                nameCounts[material.name] = count + 1;
                byMaterial[material] = new MaterialUsageInfo
                {
                    Name = material.name,
                    ShaderName = material.shader != null ? material.shader.name : "<missing shader>",
                    EnableGpuInstancing = material.enableInstancing,
                    UsesMobileShader = material.shader != null && material.shader.name.StartsWith("VRChat/Mobile/", StringComparison.Ordinal)
                };
            }

            foreach (var renderer in context.Renderers)
            {
                foreach (var material in renderer.sharedMaterials)
                {
                    if (material == null || !byMaterial.TryGetValue(material, out var info))
                    {
                        continue;
                    }

                    info.SlotCount++;
                    var path = AvatarScanner.GetPath(renderer.transform);
                    if (!info.RendererPaths.Contains(path))
                    {
                        info.RendererPaths.Add(path);
                    }
                }
            }

            foreach (var pair in byMaterial)
            {
                pair.Value.HasSameNameCandidates = nameCounts.TryGetValue(pair.Value.Name, out var count) && count > 1;
                results.Add(pair.Value);
            }

            results.Sort((a, b) => string.Compare(a.Name, b.Name, StringComparison.OrdinalIgnoreCase));
            return results;
        }
    }
}
