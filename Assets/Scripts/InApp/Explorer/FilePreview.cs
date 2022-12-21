using System.IO;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.UI;

namespace InApp.UI
{
    public class FilePreview
    {
        public string filepath;
        public bool isLoading;
        public bool isLoaded = true;

        private RawImage imageToSet;

        public FilePreview(RawImage imageToSet)
        {
            this.imageToSet = imageToSet;
        }
        public void Load()
        {
            Profiler.BeginSample("Preview");

            isLoading = true;
            isLoaded = false;

            Profiler.BeginSample("Read");
            byte[] bytes = File.ReadAllBytes(filepath);
            Profiler.EndSample();

            Profiler.BeginSample("Load");
            Texture2D tex = new Texture2D(1, 1);
            tex.LoadImage(bytes);
            Profiler.EndSample();

            imageToSet.texture = tex;

            isLoading = false;
            isLoaded = true;

            Profiler.EndSample();
        }
    }
}