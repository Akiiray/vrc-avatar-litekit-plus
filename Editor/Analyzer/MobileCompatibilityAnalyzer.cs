using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using Akiiray.VRCAvatarLiteKitPlus.Editor.Core;

namespace Akiiray.VRCAvatarLiteKitPlus.Editor.Analyzer
{
    [Serializable]
    public sealed class MobileCompatibilityWarning : DiagnosticMessage
    {
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

        private static void Add(List<MobileCompatibilityWarning> warnings, string severity, string category, string path, string code, string argument = null)
        {
            warnings.Add(new MobileCompatibilityWarning
            {
                Severity = severity,
                Category = category,
                Path = path,
                Code = code, Argument = argument
            });
        }
    }
}
