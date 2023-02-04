using InApp.UI;
using System.IO;
using UnityEngine;
using Zenject;

namespace InApp
{
    public class DownloadsWatcherUI : MonoBehaviour
    {
        [SerializeField] private Transform content;

        private DownloadsWatcher watcher;
        private DownloadUIItem.Pool pool;
        private FilesView files;

        [Inject]
        private void Construct(DownloadUIItem.Pool pool, FilesView files)
        {
            this.pool = pool;
            this.files = files;

            watcher = new DownloadsWatcher();   
        }

        private void Update()
        {
            if (watcher.changedFiles.Count > 0)
            {
                foreach (var filepath in watcher.changedFiles)
                {
                    CreateItem(filepath);
                }
                watcher.changedFiles.Clear();
            }
        }

        public void OnMoveClicked(string filepath)
        {
            string targetFilename = files.CurrentPath + "/" + Path.GetFileName(filepath);
            if (File.Exists(targetFilename) || File.Exists(filepath) == false)
            {
                Debug.LogError("Failed to move download file, no or target file path, or downloaded file was moved");
            }
            else
            {
                File.Move(filepath, targetFilename);
            }
        }
        public void OnOpenClicked(string filepath)
        {
            System.Diagnostics.Process.Start(filepath);
        }
        private void CreateItem(string filepath)
        {
            var inst = pool.Spawn(filepath);
            inst.transform.parent = content;
        }
    }
}