using UnityEngine;

namespace InApp.UI
{
    [CreateAssetMenu(menuName = "SODB/Icons")]
    public class IconsSO : ScriptableObject
    {
        public Sprite folderFill, folderEmpty, folderOpen;
        public Sprite defaultFile;
    }
}