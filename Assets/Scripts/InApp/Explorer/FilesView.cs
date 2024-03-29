using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;

namespace InApp.UI
{
    public class FilesView : MonoBehaviour, IPointerClickHandler, IDragHandler
    {
        public string CurrentPath => tab == null ? "C:/" : tab.Folder.GetFullPath();
        public int SelectedEntriesCount => selectedEntries.Count;
        public int EntriesCount => grid.models.Count;

        public Action onPathChanged;

        [SerializeField] private FilesViewMessager messager;
        [SerializeField] private EntriesGrid grid;
        [SerializeField] private ScrollRect scrollRect;
        
        private HashSet<EntryUIItemModel> selectedEntries = new HashSet<EntryUIItemModel>();
        private FileSystemWatcher fileWatcher = new FileSystemWatcher();
        private bool hasFileSystemChanges;

        private ContextMenuCreator context;
        private FileOperator fileOperator;
        private UClipboard clipboard;
        private Settings settings;
        private MouseScroll scroll;
        private BlockChecker shortcuts;

        private Tab tab;

        [Inject]
        private void Construct(ContextMenuCreator context, FileOperator fileOperator, UClipboard clipboard, Settings settings, MouseScroll scroll, BlockChecker shortcuts)
        {
            this.context = context;
            this.fileOperator = fileOperator;
            this.clipboard = clipboard;
            this.settings = settings;
            this.scroll = scroll;
            this.shortcuts = shortcuts;

            fileOperator.onAnyOperationApplied += () => FilesChanged(null, null);

            fileWatcher.Filter = "*.*";
            fileWatcher.NotifyFilter = NotifyFilters.FileName | NotifyFilters.DirectoryName;
            fileWatcher.Created += FilesChanged;
            fileWatcher.Deleted += FilesChanged;
            fileWatcher.Renamed += FilesChanged;
            fileWatcher.EnableRaisingEvents = true;

            shortcuts.Add(this);
        }

       
        private void Update()
        {
            UpdateHotkeys();

            if (hasFileSystemChanges)
            {
                RefreshWithoutAnimations();
                hasFileSystemChanges = false;
            }
        }

        public void OpenTab(Tab tab)
        {
            if (this.tab != null)
            {
                this.tab.onPathChanged -= RefreshWithAnimations;
            }

            this.tab = tab;

            this.tab.onPathChanged += RefreshWithAnimations;

            RefreshWithAnimations();
        }
        public void RefreshWithAnimations()
        {
            Show(true);
        }
        public void RefreshWithoutAnimations()
        {
            Show(false);
        }
        public void Show(bool playAnimations)
        {
            DeselectAll();

            // Handling files and folders
            var folderSortingData = settings.folderSortingData.FirstOrDefault(d => d.path == CurrentPath);

            grid.Recalculate(tab.Folder, playAnimations);

            // Updating FileWatcher
            fileWatcher.Path = CurrentPath;
            onPathChanged?.Invoke();

            messager.ClearMessage();
            scrollRect.verticalNormalizedPosition = 1;
        }
        public void ShowLoading(string message)
        {
            //grid.ClearItems();
            messager.SetMessage(message);
        }
        public void OnItemClicked(EntryUIItem item)
        {
            bool isShift = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
            bool isControl = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);

            if (isShift == false && isControl == false)
            {
                DeselectAll();
            }

            if (selectedEntries.Count > 0)
            {
                if (isShift)
                {
                    int startIndex = grid.models.IndexOf(selectedEntries.Last());
                    int targetIndex = grid.models.IndexOf(item.Model);
                    int min = Mathf.Min(targetIndex, startIndex);
                    int max = Mathf.Max(targetIndex, startIndex);

                    for (int i = min; i < max; i++)
                    {
                        SelectItem(grid.models[i]);
                    }
                }
                else if (isControl)
                {
                    if (selectedEntries.Contains(item.Model))
                    {
                        item.Model.isSelected = false;
                        selectedEntries.Remove(item.Model);
                        return;
                    }
                }
            }

            SelectItem(item.Model);
        }
        public void OnItemRightClick(EntryUIItem item)
        {
            if (selectedEntries.Contains(item.Model) == false)
            {
                DeselectAll();
                SelectItem(item.Model);
            }

            List<ContextItem> items = new List<ContextItem>()
            {
                new CreateEntryItem(),
                new RenameFileItem(),
                new CopyFileItem(UClipboard.CopyType.Copy),
                new CopyFileItem(UClipboard.CopyType.Cut),
                new PasteFileItem(),
                new DeleteFileItem(),
                new CopyEntryPathItem(item.Path, item.Entry.isFolder),
                new OpenConsoleItem(tab.Folder.GetFullPath()),
                new AddToProgramsItem(item.Path)
            };
            context.ShowMenu(items, Input.mousePosition);
        }

        public IEnumerable<string> EnumerateSelectedFIles()
        {
            foreach (EntryUIItemModel item in selectedEntries)
            {
                yield return item.entry.GetFullPathFor(tab.Folder);
            }
        }
        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                DeselectAll();
            }
            else if (eventData.button == PointerEventData.InputButton.Right)
            {
                context.ShowMenu(new List<ContextItem>()
                {
                    new CreateEntryItem(),
                    new PasteFileItem(),
                    new OpenConsoleItem(tab.Folder.GetFullPath()),
                }, Input.mousePosition);
            }
        }
        public void OnScrollRectValueChanged(Vector2 value)
        {
            scrollRect.verticalScrollbar.size = Mathf.Max(scrollRect.verticalScrollbar.size, 0.075f);
        }

        private void UpdateHotkeys()
        {
            if (shortcuts.IsPointerOverInput() || shortcuts.IsBlocked(this)) return;

            bool isControlled = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);

            // Selection
            if (selectedEntries.Count > 0)
            {
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    DeselectAll();
                }
                if (Input.GetKeyDown(KeyCode.Delete))
                {
                    DeleteSelected(Input.GetKey(KeyCode.LeftShift) == false);
                }
            }


            if (isControlled)
            {
                // File operatior undo/redo
                if (Input.GetKeyDown(KeyCode.Z)) fileOperator.Undo();
                if (Input.GetKeyDown(KeyCode.Y)) fileOperator.Redo();

                // Copy/Cut/Paste
                if (selectedEntries.Count > 0)
                {
                    if (Input.GetKeyDown(KeyCode.C)) clipboard.Copy(EnumerateSelectedFIles(), UClipboard.CopyType.Copy);
                    if (Input.GetKeyDown(KeyCode.X)) clipboard.Copy(EnumerateSelectedFIles(), UClipboard.CopyType.Cut);
                }
                if (Input.GetKeyDown(KeyCode.V)) clipboard.Paste(CurrentPath);
            }
        }
        private void SelectItem(EntryUIItemModel model)
        {
            model.isSelected = true;
            selectedEntries.Add(model);
        }
        private void DeselectAll()
        {
            foreach (var selectedItem in selectedEntries)
            {
                selectedItem.isSelected = false;
            }
            selectedEntries.Clear();
        }
        private void DeleteSelected(bool moveToBin)
        {
            foreach (EntryUIItemModel model in selectedEntries)
            {
                string path = model.entry.GetFullPathFor(tab.Folder);

                if (moveToBin)
                {
                    DeleteFileItem.FileOperationAPIWrapper.Send(path, DeleteFileItem.FileOperationAPIWrapper.FileOperationFlags.FOF_SILENT);
                }
                else
                {
                    if (Directory.Exists(path)) Directory.Delete(path, true);
                    else File.Delete(path);
                }

                model.isSelected = false;
            }

            hasFileSystemChanges = true;
            selectedEntries.Clear();
        }
        private void FilesChanged(object sender, FileSystemEventArgs e)
        {
            hasFileSystemChanges = true;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (scroll.IsScrolling == false)
            {
                scroll.Begin(OnMouseScroll);
            }
        }

        private void OnMouseScroll(float delta)
        {
            delta = Mathf.Pow(delta, 2) * Mathf.Sign(delta) / 200f;
            scrollRect.verticalNormalizedPosition = Mathf.Clamp01(scrollRect.verticalNormalizedPosition + delta / grid.ContentHeight);
        }
    }

}