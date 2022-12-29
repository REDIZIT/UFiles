using InApp.UI;
using System;
using System.IO;
using System.Runtime.InteropServices;
using TMPro;
using UnityEditor;
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

        private string folderPath;

        private FilesView files;

        [Inject]
        private void Construct(FilesView files, string path, Transform parent)
        {
            this.files = files;
            Refresh(path);
            transform.parent = parent;
        }

        public void OnClick()
        {
            files.Show(folderPath);
        }
        private void Refresh(string folderPath)
        {
            this.folderPath = folderPath;

            DirectoryInfo d = new DirectoryInfo(folderPath);
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

            var info = new DirectoryInfo(folderPath);
            label.text = info.Name;
        }
        private void RefreshAsDrive()
        {
            icon.gameObject.SetActive(false);
            circleGroup.SetActive(true);

            var info = new DriveInfo(folderPath);
            label.text = GetDriveLabel(folderPath) + " (" + info.VolumeLabel + ")";
            fillCircle.fillAmount = 1 - info.TotalFreeSpace / (float)info.TotalSize;
        }

        public const string SHELL = "shell32.dll";

        [DllImport(SHELL, CharSet = CharSet.Unicode)]
        public static extern uint SHParseDisplayName(string pszName, IntPtr zero, [Out] out IntPtr ppidl, uint sfgaoIn, [Out] out uint psfgaoOut);

        [DllImport(SHELL, CharSet = CharSet.Unicode)]
        public static extern uint SHGetNameFromIDList(IntPtr pidl, SIGDN sigdnName, [Out] out String ppszName);

        public enum SIGDN : uint
        {
            NORMALDISPLAY = 0x00000000,
            PARENTRELATIVEPARSING = 0x80018001,
            DESKTOPABSOLUTEPARSING = 0x80028000,
            PARENTRELATIVEEDITING = 0x80031001,
            DESKTOPABSOLUTEEDITING = 0x8004c000,
            FILESYSPATH = 0x80058000,
            URL = 0x80068000,
            PARENTRELATIVEFORADDRESSBAR = 0x8007c001,
            PARENTRELATIVE = 0x80080001
        }

        //var x = GetDriveLabel(@"C:\")
        public string GetDriveLabel(string driveNameAsLetterColonBackslash)
        {
            IntPtr pidl;
            uint dummy;
            string name;
            if (SHParseDisplayName(driveNameAsLetterColonBackslash.Replace("/", @"\"), IntPtr.Zero, out pidl, 0, out dummy) == 0
                && SHGetNameFromIDList(pidl, SIGDN.PARENTRELATIVEEDITING, out name) == 0
                && name != null)
            {
                return name;
            }
            return null;
        }

        public class Factory : PlaceholderFactory<string, Transform, SidebarFolder>
        {
        }
    }
}