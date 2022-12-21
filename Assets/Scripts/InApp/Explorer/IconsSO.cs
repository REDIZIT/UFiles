using UnityEngine;

namespace InApp.UI
{
    [CreateAssetMenu(menuName = "SODB/Icons")]
    public class IconsSO : ScriptableObject
    {
        public Texture2D folderFill, folderEmpty, folderOpen;
        public Texture2D defaultFile;
    }
}