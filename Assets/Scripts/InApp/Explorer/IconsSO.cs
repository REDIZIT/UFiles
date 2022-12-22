using UnityEngine;

namespace InApp.UI
{
    [CreateAssetMenu(menuName = "SODB/Icons")]
    public class IconsSO : ScriptableObject
    {
        public Texture2D folderFill, folderEmpty, folderOpen;
        public Texture2D defaultFile;

        public ContextIcons context;

        [System.Serializable]
        public class ContextIcons
        {
            public Texture2D create, rename, delete;
        }
    }
}