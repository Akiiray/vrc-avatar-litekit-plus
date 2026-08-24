using System;
using nadena.dev.ndmf;
using UnityEngine;
using VRC.SDK3.Avatars.Components;

[assembly: ExportsPlugin(typeof(Akiiray.VRCAvatarLiteKitPlus.Editor.ParameterCompression.LiteKitParameterCompressorPlugin))]

namespace Akiiray.VRCAvatarLiteKitPlus.Editor.ParameterCompression
{
    public sealed class LiteKitParameterCompressorPlugin : Plugin<LiteKitParameterCompressorPlugin>
    {
        public override string QualifiedName => "com.akiiray.vrc-avatar-litekit-plus.parameter-compressor";
        public override string DisplayName => "VRC Avatar LiteKit Parameter Compressor";

        protected override void Configure()
        {
            // VRCFury の公開 QualifiedName を確認できていないため、推測による BeforePlugin 指定は行わない。
            InPhase(BuildPhase.Optimizing).Run("同期 Parameter を時分割圧縮", context =>
            {
                var descriptor = context.AvatarRootObject.GetComponent<VRCAvatarDescriptor>();
                var compressor = context.AvatarRootObject.GetComponent<LiteKitParameterCompressor>();
                if (compressor == null || !compressor.EnableCompression) return;
                if (ParameterCompressorGenerator.Apply(context, descriptor, compressor, out var message))
                {
                    Debug.Log(message);
                    return;
                }

                // 対象なし・削減効果なしは安全なSkip。構成不備や上限超過は例外として
                // NDMFへ渡し、壊れたAnimatorを含むBuildを継続させない。
                if (message != null && (message.Contains("圧縮対象") || message.Contains("削減効果") || message.Contains("無効")))
                {
                    Debug.Log("[LiteKit Parameter Compressor] Skip: " + message);
                    return;
                }
                throw new InvalidOperationException("[LiteKit Parameter Compressor] " + message);
            });
        }
    }
}
