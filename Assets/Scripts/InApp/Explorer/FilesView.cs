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
        public int EntriesCount => spawnedItems.Count;
        public int SelectedEntriesCount => selectedEntries.Count;

        public Action onPathChanged;

        [SerializeField] private FilesViewMessager messager;
        [SerializeField] private EntriesGrid grid;
        [SerializeField] private ScrollRect scrollRect;

        private List<EntryUIItem> spawnedItems = new List<EntryUIItem>();
        private HashSet<EntryUIItem> selectedEntries = new HashSet<EntryUIItem>();
        private FileSystemWatcher fileWatcher = new FileSystemWatcher();
        private bool hasFileSystemChanges;

        private ContextMenuCreator context;
        private FileOperator fileOperator;
        private UClipboard clipboard;
        private Settings settings;
        private MouseScroll scroll;

        private Tab tab;

        [Inject]
        private void Construct(ContextMenuCreator context, FileOperator fileOperator, UClipboard clipboard, Settings settings, MouseScroll scroll)
        {
            this.context = context;
            this.fileOperator = fileOperator;
            this.clipboard = clipboard;
            this.settings = settings;
            this.scroll = scroll;

            fileOperator.onAnyOperationApplied += () => FilesChanged(null, null);

            fileWatcher.Filter = "*.*";
            fileWatcher.NotifyFilter = NotifyFilters.FileName | NotifyFilters.DirectoryName;
            fileWatcher.Created += FilesChanged;
            fileWatcher.Deleted += FilesChanged;
            fileWatcher.Renamed += FilesChanged;
            fileWatcher.EnableRaisingEvents = true;
        }

       
        private void Update()
        {
            UpdateHotkeys();

            if (hasFileSystemChanges)
            {
                Refresh();
                hasFileSystemChanges = false;
            }
        }

        public void OpenTab(Tab tab)
        {
            if (this.tab != null)
            {
                this.tab.onPathChanged -= Refresh;
            }

            this.tab = tab;

            this.tab.onPathChanged += Refresh;

            Refresh();
        }
        public void Refresh()
        {
            Show(CurrentPath);
        }
        public void Show(string path)
        {
            path = path.Replace("\\", "/").Replace(@"\", "/");

            DeselectAll();

            // Handling files and folders
            var folderSortingData = settings.folderSortingData.FirstOrDefault(d => d.path == path);

            grid.Recalculate(tab.Folder);

            // Updating FileWatcher
            //fileWatcher.Path = CurrentPath;
            onPathChanged?.Invoke();

            messager.ClearMessage();
            scrollRect.verticalNormalizedPosition = 1;
        }
        public void ShowLoading(string message)
        {
            grid.ClearItems();
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
                    int startIndex = spawnedItems.IndexOf(selectedEntries.Last());
                    int targetIndex = spawnedItems.IndexOf(item);
                    int min = Mathf.Min(targetIndex, startIndex);
                    int max = Mathf.Max(targetIndex, startIndex);

                    for (int i = min; i < max; i++)
                    {
                        SelectItem(spawnedItems[i]);
                    }
                }
                else if (isControl)
                {
                    if (selectedEntries.Contains(item))
                    {
                        item.SetSelection(false);
                        selectedEntries.Remove(item);
                        return;
                    }
                }
            }

            SelectItem(item);
        }
        public void OnItemRightClick(EntryUIItem item)
        {
            if (selectedEntries.Contains(item) == false)
            {
                DeselectAll();
                SelectItem(item);
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
                new OpenConsoleItem(tab.Folder.GetFullPath())
            };
            context.ShowMenu(items, Input.mousePosition);
        }

        public IEnumerable<string> EnumerateSelectedFIles()
        {
            foreach (EntryUIItem item in selectedEntries)
            {
                yield return item.Path;
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
        private void SelectItem(EntryUIItem item)
        {
            item.SetSelection(true);
            selectedEntries.Add(item);
        }
        private void DeselectAll()
        {
            foreach (var selectedItem in selectedEntries)
            {
                selectedItem.SetSelection(false);
            }
            selectedEntries.Clear();
        }
        private void DeleteSelected(bool moveToBin)
        {
            foreach (var entry in selectedEntries)
            {
                if (moveToBin)
                {
                    DeleteFileItem.FileOperationAPIWrapper.Send(entry.Path, DeleteFileItem.FileOperationAPIWrapper.FileOperationFlags.FOF_SILENT);
                }
                else
                {
                    if (Directory.Exists(entry.Path)) Directory.Delete(entry.Path, true);
                    else File.Delete(entry.Path);
                }
               
                entry.SetSelection(false);
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