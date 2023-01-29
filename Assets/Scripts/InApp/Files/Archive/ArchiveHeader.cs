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
        [Inject] private ArchiveViewer viewer;

        private ArchivePath path;

        private void Update()
        {
            if (viewer.IsInArchive(tabs.ActiveTab.Folder.GetFullPath()))
            {
                content.SetActive(true);
                filesScrollView.offsetMax = new Vector2(filesScrollView.offsetMax.x, -42);

                //path = tabs.ActiveTab.path.get;
                //pathText.text = Path.GetFileName(path.pathToArchive);
            }
            else
            {
                content.SetActive(false);
                filesScrollView.offsetMax = new Vector2(filesScrollView.offsetMax.x, 0);
            }
        }
        public void OnDefaultAppClicked()
        {
            System.Diagnostics.Process.Start(path.pathToArchive);
        }

        private void OnPathChanged()
        {
            
            //preview.RequestIcon(path.pathToArchive, icon);
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