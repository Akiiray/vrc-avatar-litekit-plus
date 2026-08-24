using UnityEngine;
using VRC.SDK3.Avatars.ScriptableObjects;

namespace Akiiray.VRCAvatarLiteKitPlus.Editor.ParameterCompression
{
    internal sealed class ScannedParameter
    {
        internal string Name;
        internal VRCExpressionParameters.ValueType Type;
        internal bool NetworkSynced;
        internal bool Saved;
        internal float DefaultValue;
        internal string Source;
        internal Object SourceObject;
        internal int BitCost => Type == VRCExpressionParameters.ValueType.Bool ? 1 : 8;
    }
}
