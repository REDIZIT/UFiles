using System;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace InApp.UI
{
    public class ArchiveHeader : MonoBehaviour
    {
        [SerializeField] private GameObject content;
        [SerializeField] private RectTransform filesScrollView;
        [SerializeField] private TextMeshProUGUI pathText;
        [SerializeField] private RawImage icon;

        [Inject] private TabUI tabs;
        [Inject] private FilePreview preview;

        private ArchiveFolder path;

        private void Update()
        {
            if (tabs.ActiveTab.Folder is ArchiveFolder archiveFolder)
            {
                content.SetActive(true);
                filesScrollView.offsetMax = new Vector2(filesScrollView.offsetMax.x, -42);

                path = archiveFolder;
                pathText.text = new FileInfo(archiveFolder.ArchiveFilePath).Name;
            }
            else
            {
                content.SetActive(false);
                filesScrollView.offsetMax = new Vector2(filesScrollView.offsetMax.x, 0);
            }
        }
        public void OnDefaultAppClicked()
        {
            System.Diagnostics.Process.Start(path.ArchiveFilePath);
        }
        public void OnBackClicked()
        {
            tabs.ActiveTab.TryUndoUntil(f => f is ArchiveFolder == false);
        }

        private void OnPathChanged()
        {
            preview.RequestIcon(path.ArchiveFilePath, icon);
        }
    }
    public class ExtractArchiveCommand : BridgeCommand
    {
        private string archivePath, tempFolderPath;
        private Action<string> callback;

        public ExtractArchiveCommand(string archivePath, string tempFolderPath, Action<string> callback)
        {
            this.archivePath = archivePath;
            this.tempFolderPath = tempFolderPath;
            this.callback = callback;
        }

        protected override void OnPerform()
        {
            WriteLine(archivePath);
            WriteLine(tempFolderPath);

            string message = ReadLine();
            callback(message);
        }
    }
}