using System.Collections.Generic;
using nadena.dev.ndmf;
using UnityEngine;
using VRC.SDK3.Avatars.Components;

namespace Akiiray.VRCAvatarLiteKitPlus
{
    [AddComponentMenu("VRC Avatar LiteKit Plus/Parameter Compressor")]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(VRCAvatarDescriptor))]
    public sealed class LiteKitParameterCompressor : MonoBehaviour, IEditorOnly
    {
        [SerializeField] private bool enableCompression = true;
        [SerializeField, Min(1)] private int syncChannelCount = 1;
        [SerializeField] private List<string> excludedParameters = new List<string>();

        public bool EnableCompression { get => enableCompression; set => enableCompression = value; }
        public int SyncChannelCount { get => Mathf.Max(1, syncChannelCount); set => syncChannelCount = Mathf.Max(1, value); }
        public List<string> ExcludedParameters => excludedParameters;
    }
}
