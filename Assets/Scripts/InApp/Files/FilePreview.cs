using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace InApp.UI
{
    public class FilePreview : ITickable
    {
        [Inject] private Bridge bridge;

        private Dictionary<string, Texture2D> textureByExt = new Dictionary<string, Texture2D>();
        private Dictionary<string, List<RawImage>> loadWaiters = new Dictionary<string, List<RawImage>>();
        private Dictionary<string, byte[]> loadedIcons = new Dictionary<string, byte[]>();


        public void Tick()
        {
            foreach (string ext in loadedIcons.Keys.ToArray())
            {
                Texture2D tex = new Texture2D(1, 1);
                tex.LoadImage(loadedIcons[ext]);

                textureByExt.Add(ext, tex);

                foreach (var waiter in loadWaiters[ext])
                {
                    waiter.texture = tex;
                }
                loadWaiters.Remove(ext);
            }

            loadedIcons.Clear();
        }
        public void RequestIcon(string path, RawImage image)
        {
            string ext = Path.GetExtension(path);


            if (textureByExt.TryGetValue(ext, out Texture2D texture))
            {
                // Set icon right now if we have it already loaded
                image.texture = texture;
            }
            else
            {
                // Add image to icon load waiters list
                if (loadWaiters.TryGetValue(ext, out var list))
                {
                    list.Add(image);
                }
                else
                {
                    // This request is first, so add waiter and enqueue command
                    loadWaiters.Add(ext, new List<RawImage>()
                    {
                        image
                    });

                    bridge.Enqueue(new GetFileIconCommand(ext, OnIconLoaded));
                }
            }
        }
        private void OnIconLoaded(string ext, byte[] bytes)
        {
            loadedIcons.Add(ext, bytes);
        }
    }
    public class GetFileIconCommand : BridgeCommand
    {
        private string ext;
        private Action<string, byte[]> callback;

        public GetFileIconCommand(string ext, Action<string, byte[]> callback)
        {
            this.ext = ext;
            this.callback = callback;
        }

        protected override void OnPerform()
        {
            WriteLine(ext);

            var outstream = process.StandardOutput.BaseStream;

            byte[] buffer = new byte[1024];
            outstream.Read(buffer, 0, buffer.Length);

            callback(ext, buffer);
        }
    }
}