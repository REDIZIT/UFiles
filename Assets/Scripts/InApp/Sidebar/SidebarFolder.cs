using InApp.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;

namespace InApp.Sidebar
{
    public class SidebarFolder : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private Image icon;
        [SerializeField] private TextMeshProUGUI label;
        [SerializeField] private Image fillCircle;
        [SerializeField] private GameObject circleGroup;

        private IPath path;

        private FilesView files;
        private ContextMenuCreator contextCreator;
        private TabUI tabs;
        private SidebarGroup group;

        [Inject]
        private void Construct(FilesView files, ContextMenuCreator contextCreator, TabUI tabs, SidebarGroup group, string path, Transform parent)
        {
            this.files = files;
            this.contextCreator = contextCreator;
            this.tabs = tabs;
            this.group = group;

            Refresh(new EntryPath(path));
            transform.parent = parent;
        }

        public void OnClick()
        {
            files.Show(path.GetFullPath());
        }
        private void Refresh(IPath folderPath)
        {
            path = folderPath;
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

        public void OnPointerClick(PointerEventData eventData)
        {
            if(eventData.button == PointerEventData.InputButton.Right)
            {
                contextCreator.ShowMenu(new List<ContextItem>()
                {
                    new RemoveFromFavouriteItem(path, group)
                });
            }
            else if (eventData.button == PointerEventData.InputButton.Middle)
            {
                tabs.OpenNew(path, true);
            }
        }

        public class RemoveFromFavouriteItem : ContextItem
        {
            private IPath path;
            private SidebarGroup group;

            [Inject] private Settings settings;

            public RemoveFromFavouriteItem(IPath path, SidebarGroup group)
            {
                this.path = path;
                this.group = group;
                text = "Убрать из избранного";
            }

            public override void OnClick(ContextItemEnvironment env)
            {
                base.OnClick(env);
                settings.sidebar.favourite.RemoveAll(p => p == path.GetFullPath());
                settings.Save();
                group.Refresh();
            }

            public override Texture2D GetIcon()
            {
                return icons.favourite.texture;
            }
        }
        public class Factory : PlaceholderFactory<SidebarGroup, string, Transform, SidebarFolder>
        {
        }
    }
}