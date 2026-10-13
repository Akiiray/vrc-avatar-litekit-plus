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
        [NonSerialized] public string Code;
        [NonSerialized] public string Argument;
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
                    Add(warnings, "Warning", "Shader", material.Name, "NonMobileShader", material.ShaderName);
                }

                if (!material.EnableGpuInstancing)
                {
                    Add(warnings, "Info", "Material", material.Name, "InstancingOff");
                }
            }

            AddComponents<Light>(context.AvatarRoot, warnings, "Light", "LightMobile");
            AddComponents<Camera>(context.AvatarRoot, warnings, "Camera", "CameraMobile");
            AddComponents<Cloth>(context.AvatarRoot, warnings, "Cloth", "ClothMobile");
            AddComponents<AudioSource>(context.AvatarRoot, warnings, "AudioSource", "AudioMobile");
            AddComponents<Rigidbody>(context.AvatarRoot, warnings, "Rigidbody", "RigidbodyMobile");
            AddComponents<Collider>(context.AvatarRoot, warnings, "Collider", "ColliderMobile");
            AddComponents<Joint>(context.AvatarRoot, warnings, "Joint", "JointMobile");
            AddComponents<IConstraint>(context.AvatarRoot, warnings, "Unity Constraint", "ConstraintMobile");
            AddComponents<ParticleSystem>(context.AvatarRoot, warnings, "ParticleSystem", "ParticleMobile");
            AddComponents<TrailRenderer>(context.AvatarRoot, warnings, "TrailRenderer", "TrailMobile");
            AddComponents<LineRenderer>(context.AvatarRoot, warnings, "LineRenderer", "LineMobile");

            return warnings;
        }

        private static void AddComponents<T>(GameObject root, List<MobileCompatibilityWarning> warnings, string category, string code)
        {
            var components = root.GetComponentsInChildren<T>(true);
            foreach (var component in components)
            {
                if (component is Component unityComponent)
                {
                    Add(warnings, "Warning", category, AvatarScanner.GetPath(unityComponent.transform), code);
                }
            }
        }

        // Stable English text for the pre-PR Copy JSON contract, separate from UI translation.
        private static string LegacyMessage(string code, string argument)
        {
            switch (code)
            {
                case "NonMobileShader": return $"Material uses non VRChat/Mobile shader: {argument}";
                case "InstancingOff": return "GPU Instancing is OFF. This is a recommendation only; LiteKit will not modify the material.";
                case "LightMobile": return "Lights are removed or unsupported on mobile avatars.";
                case "CameraMobile": return "Cameras may be incompatible with mobile avatar targets.";
                case "ClothMobile": return "Cloth is not supported for mobile avatar display.";
                case "AudioMobile": return "Audio Sources are removed on mobile avatars.";
                case "RigidbodyMobile": return "Physics Rigidbodies are limited on mobile avatars.";
                case "ColliderMobile": return "Physics Colliders are limited on mobile avatars.";
                case "JointMobile": return "Joints can be problematic for mobile avatars.";
                case "ConstraintMobile": return "Unity constraints count toward constraint limits.";
                case "ParticleMobile": return "Particle Systems are heavily limited on mobile avatars.";
                case "TrailMobile": return "Trail Renderers are limited on mobile avatars.";
                case "LineMobile": return "Line Renderers are limited on mobile avatars.";
                default: return code;
            }
        }

        private static void Add(List<MobileCompatibilityWarning> warnings, string severity, string category, string path, string code, string argument = null)
        {
            warnings.Add(new MobileCompatibilityWarning
            {
                Severity = severity,
                Category = category,
                Path = path,
                Message = LegacyMessage(code, argument),
                Code = code, Argument = argument
            });
        }
    }
}
