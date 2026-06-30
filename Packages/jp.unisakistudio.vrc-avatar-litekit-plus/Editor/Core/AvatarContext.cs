using UnityEngine;

namespace jp.unisakistudio.vrcavatarlitekitplus.Editor.Core
{
    /// <summary>
    /// Non-destructive analysis context for a selected avatar root.
    /// </summary>
    public sealed class AvatarContext
    {
        public AvatarContext(GameObject avatarRoot)
        {
            AvatarRoot = avatarRoot;
        }

        public GameObject AvatarRoot { get; }
        public bool IsValid => AvatarRoot != null;
    }
}
