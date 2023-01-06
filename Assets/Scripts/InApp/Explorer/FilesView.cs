using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace InApp.UI
{
    public class FilesView : MonoBehaviour, IPointerClickHandler
    {
        public string CurrentPath => tab == null ? "C:/" : tab.path.GetFullPath();
        public int EntriesCount => entries.Count;
        public int SelectedEntriesCount => selectedEntries.Count;

        public Action onPathChanged;

        [SerializeField] private Transform content;

        private List<EntryUIItem> entries = new List<EntryUIItem>();
        private HashSet<EntryUIItem> selectedEntries = new HashSet<EntryUIItem>();
        private FileSystemWatcher fileWatcher = new FileSystemWatcher();
        private bool hasFileSystemChanges;
        private History<string> history = new History<string>(HistoryPointerType.TargetFrame);

        private EntryUIItem.Pool pool;
        private ContextMenuCreator context;
        private FileOperator fileOperator;
        private UClipboard clipboard;
        private Settings settings;

        private Tab tab;

        [Inject]
        private void Construct(EntryUIItem.Pool pool, ContextMenuCreator context, FileOperator fileOperator, UClipboard clipboard, Settings settings)
        {
            this.pool = pool;
            this.context = context;
            this.fileOperator = fileOperator;
            this.clipboard = clipboard;
            this.settings = settings;

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
                Show(CurrentPath, false);
                hasFileSystemChanges = false;
            }
        }

        public void OpenTab(Tab tab)
        {
            this.tab = tab;
            Refresh();
        }
        public void Refresh()
        {
            Show(tab.path.GetFullPath(), false);
        }
        public void Show(string path, bool saveToHistory = true)
        {
            path = path.Replace("\\", "/").Replace(@"\", "/");
            tab.path.Set(path);

            if (saveToHistory)
            {
                history.Add(path);
            }

            DeselectAll();


            // Handling files and folders
            var folderSortingData = settings.folderSortingData.FirstOrDefault(d => d.path == path);
            IEnumerable<string> folderEnties = Directory.GetDirectories(path).Union(Directory.GetFiles(path));

            folderEnties.Where(p =>
            {
                if (Path.GetExtension(p) == ".meta")
                {
                    string metaTargetFile = p.Substring(0, p.Length - ".meta".Length);
                    if (EntryUtils.Exists(metaTargetFile)) return false;
                }
                return true;
            });

            System.Diagnostics.Stopwatch w = System.Diagnostics.Stopwatch.StartNew();
            folderEnties = EntriesSorter.Sort(folderEnties, folderSortingData == null ? FolderSortingData.Type.None : folderSortingData.type);
            if (folderSortingData != null && folderSortingData.isReversed)
            {
                folderEnties = folderEnties.Reverse();
            }
            w.Stop();
            Debug.Log("Sorted in " + w.ElapsedMilliseconds + "ms");

            // Spawning and despawning UIItems
            int spawnedCount = entries.Count;
            int targetCount = folderEnties.Count();

            if (spawnedCount > targetCount)
            {
                for (int i = 0; i < spawnedCount - targetCount; i++)
                {
                    pool.Despawn(entries[0]);
                    entries.RemoveAt(0);
                }
            }
            else if (spawnedCount < targetCount)
            {
                for (int i = 0; i < targetCount - spawnedCount; i++)
                {
                    SpawnItem();
                }
            }

            // Refreshing UIItems
            int j = 0;
            foreach (var entry in folderEnties)
            {
                entries[j].Refresh(entry);
                j++;
            }

            
            // Updating FileWatcher
            fileWatcher.Path = CurrentPath;
            onPathChanged?.Invoke();
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
                    int startIndex = entries.IndexOf(selectedEntries.Last());
                    int targetIndex = entries.IndexOf(item);
                    int min = Mathf.Min(targetIndex, startIndex);
                    int max = Mathf.Max(targetIndex, startIndex);

                    for (int i = min; i < max; i++)
                    {
                        SelectItem(entries[i]);
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
                }, Input.mousePosition);
            }
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

            // History
            if (Input.GetMouseButtonDown(3) && history.TryUndo(out string directory))
            {
                Show(directory, false);
            }
            if (Input.GetMouseButtonDown(4) && history.TryRedo(out directory))
            {
                Show(directory, false);
            }
            if (isControlled)
            {
                if (Input.GetKeyDown(KeyCode.Z)) fileOperator.Undo();
                if (Input.GetKeyDown(KeyCode.Y)) fileOperator.Redo();
            }

            // Copy/paste
            if (isControlled)
            {
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
        private void SpawnItem()
        {
            var item = pool.Spawn();
            item.transform.parent = content;
            item.transform.SetAsLastSibling();

            entries.Add(item);
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
    }

}