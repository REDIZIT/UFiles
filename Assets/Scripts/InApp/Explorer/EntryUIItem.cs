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
        public string Path { get; private set; }

        [SerializeField] private TextMeshProUGUI name, size, modifyDate;
        [SerializeField] private RawImage icon;
        [SerializeField] private Image background;

        [SerializeField] private Color defaultColor, hoverColor, selectedColor;

        [SerializeField] private RectTransform linkedFilesContent, linkedFilesGroup;
        [SerializeField] private GameObject linkButton;
        [SerializeField] private RectTransform linkArrow;

        [SerializeField] private GameObject selectionIndicator;

        private bool isHovered, isLinkedHovered, isLinkedExpanded;
        private List<EntryUIItem> linkedItems = new();
        private EntryUIItem linkedParent;
        private RectTransform rect;
        private DateTime lastClickTime;
        private FilePreview preview;

        private IconsSO icons;
        private FilesView files;
        private Pool pool;
        private ContextMenuCreator contextCreator;
        private CreateRenameWindow createWindow;

        private const int DOUBLE_CLICK_MS = 250;

        [Inject]
        private void Construct(IconsSO icons, FilesView view, Pool pool, ContextMenuCreator contextCreator, CreateRenameWindow createWindow)
        {
            this.icons = icons;
            this.files = view;
            this.pool = pool;
            this.contextCreator = contextCreator;
            this.createWindow = createWindow;
            rect = GetComponent<RectTransform>();
            preview = new FilePreview(icon);
        }

        private void Update()
        {
            if (preview.isLoading == false && preview.isLoaded == false)
            {
                preview.Load(Path);
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                files.OnItemClicked(this);

                if ((DateTime.Now - lastClickTime).TotalMilliseconds <= DOUBLE_CLICK_MS)
                {
                    Open();
                }
                lastClickTime = DateTime.Now;
            }
            else if (eventData.button == PointerEventData.InputButton.Right)
            {
                files.OnItemRightClick(this);

                List<ContextItem> items = new()
                {
                    new CreateEntryItem(),
                    new DeleteFileItem(files),
                    new RenameItem(),
                };
                contextCreator.ShowMenu(items, Input.mousePosition);
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
            Path = path;

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
                        icon.texture = icons.folderFill;
                    }
                    else
                    {
                        icon.texture = icons.folderEmpty;
                    }
                }
                catch(UnauthorizedAccessException)
                {
                    name.text += " [нет доступа]";
                    size.text = "нет доступа";
                    icon.texture = icons.folderEmpty;
                }
            }
            else
            {
                icon.texture = icons.defaultFile;
                FileInfo info = new FileInfo(path);
                name.text = info.Name;
                size.text = FileSizeUtil.BytesToString(info.Length);
                modifyDate.text = FileSizeUtil.PrettyModifyDate(info.LastWriteTime);
            }

            CheckAndCreateLinkedFiles();
            UpdateExpandLinkedItems();

            UpdateColor();

            if (preview.CanHandle(path))
            {
                preview.filepath = path;
                preview.isLoading = false;
                preview.isLoaded = false;
            }
        }
        private void CheckAndCreateLinkedFiles()
        {
            bool hasLinkedItems = File.Exists(Path + ".meta");

            if (hasLinkedItems)
            {
                CreateLinkedItem(Path + ".meta");
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
            FileAttributes attr = File.GetAttributes(Path);
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
                files.Show(Path);
            }
            else
            {
                Process.Start(Path);
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