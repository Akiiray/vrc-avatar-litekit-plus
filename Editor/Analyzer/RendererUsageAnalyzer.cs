using System;
using System.Collections.Generic;
using UnityEngine;
using Akiiray.VRCAvatarLiteKitPlus.Editor.Core;

namespace Akiiray.VRCAvatarLiteKitPlus.Editor.Analyzer
{
    [Serializable]
    public sealed class RendererUsageInfo
    {
        public string Name;
        public string Path;
        public string RendererType;
        public int MaterialSlotCount;
        public string MeshName;
        public int TriangleCount;
        public bool Enabled;
        public bool GameObjectActiveInHierarchy;
        public bool IsSkinnedMeshRenderer;
        public int BlendShapeCount;
        public int BoneCount;
    }

    public sealed class RendererUsageAnalyzer
    {
        public List<RendererUsageInfo> Analyze(AvatarContext context)
        {
            var results = new List<RendererUsageInfo>();
            if (context == null || !context.IsValid)
            {
                return results;
            }

            foreach (var renderer in context.Renderers)
            {
                var mesh = GetMesh(renderer);
                var info = new RendererUsageInfo
                {
                    Name = renderer.name,
                    Path = AvatarScanner.GetPath(renderer.transform),
                    RendererType = renderer.GetType().Name,
                    MaterialSlotCount = renderer.sharedMaterials.Length,
                    MeshName = mesh != null ? mesh.name : "<none>",
                    TriangleCount = CountTriangles(mesh),
                    Enabled = renderer.enabled,
                    GameObjectActiveInHierarchy = renderer.gameObject.activeInHierarchy,
                    IsSkinnedMeshRenderer = renderer is SkinnedMeshRenderer,
                    BlendShapeCount = mesh != null ? mesh.blendShapeCount : 0,
                    BoneCount = renderer is SkinnedMeshRenderer skinned && skinned.bones != null ? skinned.bones.Length : 0
                };
                results.Add(info);
            }

            return results;
        }

        private static Mesh GetMesh(Renderer renderer)
        {
            if (renderer is SkinnedMeshRenderer skinned)
            {
                return skinned.sharedMesh;
            }

            var meshFilter = renderer.GetComponent<MeshFilter>();
            return meshFilter != null ? meshFilter.sharedMesh : null;
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
