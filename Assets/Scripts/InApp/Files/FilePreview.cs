using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace InApp.UI
{
    public class FilePreview
    {
        public string filepath;

        private RawImage imageToSet;

        public FilePreview(RawImage imageToSet)
        {
            this.imageToSet = imageToSet;
        }

        public bool CanHandle(string path)
        {
            string ext = Path.GetExtension(path);
            return ext == ".png" || ext == ".jpg";
        }
        public void Load(string filepath)
        {
            this.filepath = filepath;

            byte[] bytes = File.ReadAllBytes(filepath);

            Texture2D tex = new Texture2D(1, 1);
            tex.LoadImage(bytes);

            imageToSet.texture = tex;
        }
        public Sprite GetSprite()
        {
            return Sprite.Create((Texture2D)imageToSet.texture, new Rect(0, 0, imageToSet.texture.width, imageToSet.texture.height), Vector2.one / 2f);
        }
    }
}