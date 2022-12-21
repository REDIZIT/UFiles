using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;

namespace InApp.UI
{
    public class EntryUIItem : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        public bool IsSelected { get; private set; }

        [SerializeField] private TextMeshProUGUI name, size, modifyDate;
        [SerializeField] private Image icon;
        [SerializeField] private Image background;

        [SerializeField] private Color defaultColor, hoverColor, selectedColor;

        [SerializeField] private RectTransform linkedFilesContent, linkedFilesGroup;
        [SerializeField] private GameObject linkButton;
        [SerializeField] private RectTransform linkArrow;

        [SerializeField] private GameObject selectionIndicator;

        private bool isHovered, isLinkedHovered, isLinkedExpanded;
        private string path;
        private List<EntryUIItem> linkedItems = new();
        private EntryUIItem linkedParent;
        private RectTransform rect;
        private DateTime lastClickTime;

        private IconsSO icons;
        private FilesView view;
        private Pool pool;

        private const int DOUBLE_CLICK_MS = 250;

        [Inject]
        private void Construct(IconsSO icons, FilesView view, Pool pool)
        {
            this.icons = icons;
            this.view = view;
            this.pool = pool;
            rect = GetComponent<RectTransform>();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            view.OnItemClicked(this);

            if (eventData.button == PointerEventData.InputButton.Left)
            {
                if ((DateTime.Now - lastClickTime).TotalMilliseconds <= DOUBLE_CLICK_MS)
                {
                    Open();
                }
                lastClickTime = DateTime.Now;
            }
        }

        

        public void OnPointerExit(PointerEventData eventData)
        {
            isHovered = false;
            UpdateColor();

            if (linkedParent != null)
            {
                linkedParent.OnLinkedItemExit();
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            isHovered = true;
            UpdateColor();

            if (linkedParent != null)
            {
                linkedParent.OnLinkedItemEnter();
            }
        }

        public void ClickExpandLinkedItems()
        {
            isLinkedExpanded = !isLinkedExpanded;
            UpdateExpandLinkedItems();
        }
        public void SetSelection(bool isSelected)
        {
            IsSelected = isSelected;
            selectionIndicator.SetActive(isSelected);
            UpdateColor();
        }


        private void Refresh(string path)
        {
            this.path = path;

            if (IsPathDirectory())
            {
                DirectoryInfo info = new DirectoryInfo(path);
                name.text = info.Name;
                modifyDate.text = FileSizeUtil.PrettyModifyDate(info.LastWriteTime);
                
                try
                {
                    int entriesCount = Directory.GetFileSystemEntries(path).Length;
                    size.text = entriesCount + " элементов";
                    if (entriesCount > 0)
                    {
                        icon.sprite = icons.folderFill;
                    }
                    else
                    {
                        icon.sprite = icons.folderEmpty;
                    }
                }
                catch(UnauthorizedAccessException)
                {
                    name.text += " [нет доступа]";
                    size.text = "нет доступа";
                    icon.sprite = icons.folderEmpty;
                }
            }
            else
            {
                icon.sprite = icons.defaultFile;
                FileInfo info = new FileInfo(path);
                name.text = info.Name;
                size.text = FileSizeUtil.BytesToString(info.Length);
                modifyDate.text = FileSizeUtil.PrettyModifyDate(info.LastWriteTime);
            }

            CheckAndCreateLinkedFiles();
            UpdateExpandLinkedItems();

            UpdateColor();
        }
        private void CheckAndCreateLinkedFiles()
        {
            bool hasLinkedItems = File.Exists(path + ".meta");

            if (hasLinkedItems)
            {
                CreateLinkedItem(path + ".meta");
            }

            linkButton.SetActive(hasLinkedItems);
            linkedFilesGroup.gameObject.SetActive(hasLinkedItems);
        }
        private void CreateLinkedItem(string filepath)
        {
            var inst = pool.Spawn(filepath);
            inst.transform.parent = linkedFilesContent;
            inst.linkedParent = this;
            linkedItems.Add(inst);

            inst.UpdateColor();
        }
        private void UpdateExpandLinkedItems()
        {
            linkArrow.eulerAngles = new Vector3(0, 0, isLinkedExpanded ? 90 : -90);
            linkedFilesGroup.gameObject.SetActive(isLinkedExpanded);
            FitSize();
        }

        private bool IsPathDirectory()
        {
            FileAttributes attr = File.GetAttributes(path);
            return (attr & FileAttributes.Directory) == FileAttributes.Directory;
        }
        private void FitSize()
        {
            float height = 32;
            float linkedHeight = linkedFilesContent.childCount * 32;
            if (isLinkedExpanded)
            {
                height += linkedHeight;
            }
            rect.sizeDelta = new Vector2(rect.sizeDelta.x, height);
            linkedFilesGroup.sizeDelta = new Vector2(linkedFilesGroup.sizeDelta.x, linkedHeight);
        }
        private void OnLinkedItemEnter()
        {
            isLinkedHovered = true;
            OnPointerExit(null);
        }
        private void OnLinkedItemExit()
        {
            isLinkedHovered = false;
            OnPointerEnter(null);
        }
        private void UpdateColor()
        {
            background.color = GetBackgroundColor();
        }
        private Color GetBackgroundColor()
        {
            if (isHovered && isLinkedHovered == false) return hoverColor;
            if (IsSelected) return selectedColor;

            return linkedParent == null ? defaultColor : Color.clear;
        }
        private void Open()
        {
            if (IsPathDirectory())
            {
                view.Show(path);
            }
            else
            {
                Process.Start(path);
            }
        }

        public class Pool : MonoMemoryPool<string, EntryUIItem>
        {
            protected override void Reinitialize(string p1, EntryUIItem item)
            {
                item.linkedParent = null;
                item.isHovered = false;
                item.isLinkedHovered = false;
                item.isLinkedExpanded = false;
                item.SetSelection(false);

                item.Refresh(p1);

                base.Reinitialize(p1, item);
            }
            protected override void OnDespawned(EntryUIItem item)
            {
                foreach (EntryUIItem linkedItem in item.linkedItems)
                {
                    Despawn(linkedItem);
                }
                item.linkedItems.Clear();

                base.OnDespawned(item);
            }
        }
    }
}