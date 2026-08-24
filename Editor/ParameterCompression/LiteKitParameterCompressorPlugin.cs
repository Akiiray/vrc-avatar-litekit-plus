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
                if (ParameterCompressorGenerator.Apply(descriptor, compressor, out var message)) Debug.Log(message);
                else Debug.LogWarning("[LiteKit Parameter Compressor] " + message);
            });
        }
    }
}
