using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using Akiiray.VRCAvatarLiteKitPlus.Editor.Core;

namespace Akiiray.VRCAvatarLiteKitPlus.Editor.Analyzer
{
    [Serializable]
    public sealed class MobileCompatibilityWarning
    {
        public string Severity;
        public string Category;
        public string Path;
        public string Message;
    }

    public sealed class MobileCompatibilityAnalyzer
    {
        public List<MobileCompatibilityWarning> Analyze(AvatarContext context, IReadOnlyList<MaterialUsageInfo> materials)
        {
            var warnings = new List<MobileCompatibilityWarning>();
            if (context == null || !context.IsValid)
            {
                return warnings;
            }

            foreach (var material in materials)
            {
                // v0.1 heuristic only: VRChat mobile compatibility is approximated by shader name prefix.
                if (!material.UsesMobileShader)
                {
                    Add(warnings, "Warning", "Shader", material.Name, $"Material uses non VRChat/Mobile shader: {material.ShaderName}");
                }

                if (!material.EnableGpuInstancing)
                {
                    Add(warnings, "Info", "Material", material.Name, "GPU Instancing is OFF. This is a recommendation only; LiteKit will not modify the material.");
                }
            }

            AddComponents<Light>(context.AvatarRoot, warnings, "Light", "Lights are removed or unsupported on mobile avatars.");
            AddComponents<Camera>(context.AvatarRoot, warnings, "Camera", "Cameras may be incompatible with mobile avatar targets.");
            AddComponents<Cloth>(context.AvatarRoot, warnings, "Cloth", "Cloth is not supported for mobile avatar display.");
            AddComponents<AudioSource>(context.AvatarRoot, warnings, "AudioSource", "Audio Sources are removed on mobile avatars.");
            AddComponents<Rigidbody>(context.AvatarRoot, warnings, "Rigidbody", "Physics Rigidbodies are limited on mobile avatars.");
            AddComponents<Collider>(context.AvatarRoot, warnings, "Collider", "Physics Colliders are limited on mobile avatars.");
            AddComponents<Joint>(context.AvatarRoot, warnings, "Joint", "Joints can be problematic for mobile avatars.");
            AddComponents<IConstraint>(context.AvatarRoot, warnings, "Unity Constraint", "Unity constraints count toward constraint limits.");
            AddComponents<ParticleSystem>(context.AvatarRoot, warnings, "ParticleSystem", "Particle Systems are heavily limited on mobile avatars.");
            AddComponents<TrailRenderer>(context.AvatarRoot, warnings, "TrailRenderer", "Trail Renderers are limited on mobile avatars.");
            AddComponents<LineRenderer>(context.AvatarRoot, warnings, "LineRenderer", "Line Renderers are limited on mobile avatars.");

            return warnings;
        }

        private static void AddComponents<T>(GameObject root, List<MobileCompatibilityWarning> warnings, string category, string message)
        {
            var components = root.GetComponentsInChildren<T>(true);
            foreach (var component in components)
            {
                if (component is Component unityComponent)
                {
                    Add(warnings, "Warning", category, AvatarScanner.GetPath(unityComponent.transform), message);
                }
            }
        }

        private static void Add(List<MobileCompatibilityWarning> warnings, string severity, string category, string path, string message)
        {
            warnings.Add(new MobileCompatibilityWarning
            {
                Severity = severity,
                Category = category,
                Path = path,
                Message = message
            });
        }
    }
}
