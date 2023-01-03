using InApp.UI;
using System;
using System.IO;
using System.Runtime.InteropServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace InApp.Sidebar
{
    public class SidebarFolder : MonoBehaviour
    {
        [SerializeField] private Image icon;
        [SerializeField] private TextMeshProUGUI label;
        [SerializeField] private Image fillCircle;
        [SerializeField] private GameObject circleGroup;

        private IPath path;

        private FilesView files;

        [Inject]
        private void Construct(FilesView files, string path, Transform parent)
        {
            this.files = files;
            Refresh(new EntryPath(path));
            transform.parent = parent;
        }

        public void OnClick()
        {
            files.Show(path.GetFullPath());
        }
        private void Refresh(IPath folderPath)
        {
            this.path = folderPath;
            label.text = folderPath.GetDisplayName();

            DirectoryInfo d = new DirectoryInfo(folderPath.GetFullPath());
            if (d.Parent == null)
            {
                RefreshAsDrive();
            }
            else
            {
                RefreshAsFolder();
            }
        }
        private void RefreshAsFolder()
        {
            icon.gameObject.SetActive(true);
            circleGroup.SetActive(false);
        }
        private void RefreshAsDrive()
        {
            icon.gameObject.SetActive(false);
            circleGroup.SetActive(true);

            var info = new DriveInfo(path.GetFullPath());
            fillCircle.fillAmount = 1 - info.TotalFreeSpace / (float)info.TotalSize;
        }


        

        public class Factory : PlaceholderFactory<string, Transform, SidebarFolder>
        {
        }
    }
}