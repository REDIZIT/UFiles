using UnityEngine;

namespace InApp.UI
{
    [CreateAssetMenu(menuName = "SODB/Icons")]
    public class IconsSO : ScriptableObject
    {
        public Sprite folderFill, folderEmpty, folderOpen;
        public Sprite defaultFile;

        public ContextIcons context;
        public PathBarIcons pathBar;

        public Sprite favourite;

        [System.Serializable]
        public class ContextIcons
        {
            public Sprite create, rename, delete;
            public Sprite copy, cut, paste;
        }

        [System.Serializable]
        public class PathBarIcons
        {
            public Sprite subFolder, systemFolder, userFolder;
        }
    }
}