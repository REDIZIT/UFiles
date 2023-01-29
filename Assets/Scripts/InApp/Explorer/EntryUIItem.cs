using System;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;

namespace InApp.UI
{
    public class EntryUIItem : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler, IDragHandler
    {
        public bool IsSelected { get; private set; }
        public Entry Entry { get; private set; }
        public string Path => Entry.GetFullPathFor(tabs.ActiveTab.Folder);

        [SerializeField] private TextMeshProUGUI name, size, modifyDate;
        [SerializeField] private RawImage icon;
        [SerializeField] private Image background;

        [SerializeField] private Color defaultColor, hoverColor, selectedColor;

        [SerializeField] private RectTransform linkedFilesContent, linkedFilesGroup;
        [SerializeField] private GameObject linkButton;
        [SerializeField] private RectTransform linkArrow;

        [SerializeField] private GameObject selectionIndicator;
        [SerializeField] private PointerHandler previewImage;

        private bool isHovered, isLinkedHovered, isLinkedExpanded;
        private List<EntryUIItem> linkedItems = new List<EntryUIItem>();    
        private EntryUIItem linkedParent;
        private RectTransform rect;
        private DateTime lastClickTime;
        private FilePreview preview;
        private AppDragDrop dragDrop;
        private PicturePreview picturePreview;

        private IconsSO icons;
        private FilesView files;
        private Pool pool;
        private ContextMenuCreator contextCreator;
        private TabUI tabs;

        private const int DOUBLE_CLICK_MS = 250;

        [Inject]
        private void Construct(IconsSO icons, FilesView files, Pool pool, ContextMenuCreator contextCreator, TabUI tabs, AppDragDrop dragDrop, PicturePreview picturePreview, FilePreview preview)
        {
            this.icons = icons;
            this.files = files;
            this.pool = pool;
            this.contextCreator = contextCreator;
            this.tabs = tabs;
            this.dragDrop = dragDrop;
            this.picturePreview = picturePreview;
            this.preview = preview;

            rect = GetComponent<RectTransform>();

            BindPointerHandlers();
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

                List<ContextItem> items = new List<ContextItem>()
                {
                    new CreateEntryItem(),
                    new RenameFileItem(),
                    new CopyFileItem(UClipboard.CopyType.Copy),
                    new CopyFileItem(UClipboard.CopyType.Cut),
                    new PasteFileItem(),
                    new DeleteFileItem(),
                };
                contextCreator.ShowMenu(items, Input.mousePosition);
            }
            else if (eventData.button == PointerEventData.InputButton.Middle)
            {
                bool switchToNew = Input.GetKey(KeyCode.LeftShift) == false;
                tabs.OpenNew(new LocalFolder(Path), switchToNew);
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


        public void Refresh(Entry entry)
        {
            linkedParent = null;
            isHovered = false;
            isLinkedHovered = false;
            isLinkedExpanded = false;
            SetSelection(false);

            Entry = entry;

            if (entry.isFolder)
            {
                name.text = entry.name;
                modifyDate.text = FileSizeUtil.PrettyModifyDate(entry.lastWriteTime);

                long entriesCount = entry.size;
                if (entriesCount == -1)
                {
                    name.text += " [��� �������]";
                    size.text = "��� �������";
                    icon.texture = icons.folderEmpty.texture;
                }
                else if (entriesCount > 0)
                {
                    icon.texture = icons.folderFill.texture;
                    size.text = entriesCount + " ���������";
                }
                else
                {
                    icon.texture = icons.folderEmpty.texture;
                    size.text = "��� ���������";
                }
            }
            else
            {
                icon.texture = icons.defaultFile.texture;
                name.text = entry.name;
                size.text = FileSizeUtil.BytesToString(entry.size);
                modifyDate.text = FileSizeUtil.PrettyModifyDate(entry.lastWriteTime);
            }

            ClearLinkedItems();
            CheckAndCreateLinkedFiles();
            UpdateExpandLinkedItems();

            UpdateColor();

            if (entry.isFolder == false)
            {
                preview.RequestIcon(Path, icon);
            }
        }
        private void BindPointerHandlers()
        {
            previewImage.onPointerEnter += () =>
            {
                picturePreview.TryOpen(Path, previewImage.Rect);
            };
            previewImage.onPointerExit += () =>
            {
                picturePreview.Hide();
            };
        }
        private void ClearLinkedItems()
        {
            foreach (EntryUIItem linkedItem in linkedItems)
            {
                pool.Despawn(linkedItem);
            }
            linkedItems.Clear();
        }
        private void CheckAndCreateLinkedFiles()
        {
            Debug.Log("Linked items disabled");
            //bool hasLinkedItems = File.Exists(Path + ".meta");

            //if (hasLinkedItems)
            //{
            //    CreateLinkedItem(Path + ".meta");
            //}

            //linkButton.SetActive(hasLinkedItems);
            //linkedFilesGroup.gameObject.SetActive(hasLinkedItems);
        }
        private void CreateLinkedItem(Entry entry)
        {
            var inst = pool.Spawn();
            inst.Refresh(entry);
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
            if (IsPathDirectory() || ArchiveViewer.IsArchive(Path))
            {
                tabs.ActiveTab.Open(Entry);
            }
            else
            {
                System.Diagnostics.Process.Start(Path);
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (dragDrop.IsDragging == false)
            {
                dragDrop.StartDrag(new EntryData()
                {
                    path = Path,
                    icon = icon.texture.ToSprite()
                });
            }
        }

        public class Pool : MonoMemoryPool<EntryUIItem>
        {
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
        public class EntryData : DragDropData
        {
            public string path;
            public Sprite icon;

            public override Sprite GetIcon()
            {
                return icon;
            }
            public override string GetDisplayText()
            {
                return EntryUtils.GetName(path);
            }
        }
    }
}