using System.Collections.Generic;
using System.IO;
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
        public Sprite browser;

        public Dictionary<string, Sprite> customIcons = new Dictionary<string, Sprite>();

        [System.Serializable]
        public class ContextIcons
        {
            public Sprite create, rename, delete;
            public Sprite copy, cut, paste;
            public Sprite cmd;
            public Sprite list;
        }

        [System.Serializable]
        public class PathBarIcons
        {
            public Sprite subFolder, systemFolder, userFolder;
            public Sprite projectSubFolder, project;
        }

        public void LoadCustomIcons()
        {
            string folder = Application.persistentDataPath + "/Icons";
            if (Directory.Exists(folder) == false) Directory.CreateDirectory(folder);

            customIcons.Clear();
            foreach (string file in Directory.GetFiles(folder))
            {
                byte[] bytes = File.ReadAllBytes(file);
                Texture2D tex = new Texture2D(1, 1);
                tex.LoadImage(bytes);
                
                string name = Path.GetFileNameWithoutExtension(file);
                customIcons.Add(name, tex.ToSprite());
            }
        }
    }
}