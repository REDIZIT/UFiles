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
        [SerializeField] private TextMeshProUGUI pathText, actionText;
        [SerializeField] private RawImage icon;

        [SerializeField] private GameObject defaultRightSide, extractRightSide;

        [Inject] private TabUI tabs;
        [Inject] private FilePreview preview;
        [Inject] private FileOperator fileOperator;

        private ArchiveFolder archive;
        private bool isSelectingExtractFolder;

        private void Start()
        {
            tabs.onActiveTabPathChanged += OnPathChanged;
        }
        private void OnDestroy()
        {
            tabs.onActiveTabPathChanged -= OnPathChanged;
        }
        public void OnDefaultAppClicked()
        {
            System.Diagnostics.Process.Start(archive.ArchiveFilePath);
        }
        public void OnBackClicked()
        {
            if (isSelectingExtractFolder)
            {
                isSelectingExtractFolder = false;
            }
            else
            {
                ClearHistory();
            }
        }
        public void OnExtractClicked()
        {
            isSelectingExtractFolder = true;

            ClearHistory();
        }
        public void OnExtractConfirmClicked()
        {
            archive.Close();

            string source = archive.ExtractedFolderPath;
            string dest = tabs.ActiveTab.Folder.GetFullPath();
            fileOperator.Run(new FolderMoveOperation(source, dest, FolderMoveOperation.Type.ContentWithFolderRemove));
            isSelectingExtractFolder = false;
        }

        private void UpdateDisabled()
        {
            content.SetActive(false);
            filesScrollView.offsetMax = new Vector2(filesScrollView.offsetMax.x, 0);
        }
        private void UpdateEnabled(bool isExtractionState)
        {
            content.SetActive(true);
            filesScrollView.offsetMax = new Vector2(filesScrollView.offsetMax.x, -42);

            defaultRightSide.SetActive(isExtractionState == false);
            extractRightSide.SetActive(isExtractionState);
        }
        private void ClearHistory()
        {
            tabs.ActiveTab.TryUndoUntil(f => f is ArchiveFolder == false);
        }
        private void OnPathChanged()
        {
            Debug.Log("On path changed");

            if (isSelectingExtractFolder)
            {
                UpdateEnabled(true);

                actionText.text = "���������� ������";
            }
            else if (tabs.ActiveTab.Folder is ArchiveFolder archiveFolder)
            {
                UpdateEnabled(false);

                archive = archiveFolder;
                pathText.text = new FileInfo(archiveFolder.ArchiveFilePath).Name;
                actionText.text = "�������� ������";
                preview.RequestIcon(archive.ArchiveFilePath, icon);
            }
            else
            {
                UpdateDisabled();
            }
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