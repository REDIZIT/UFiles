using System;
using System.Collections.Generic;
using Stopwatch = System.Diagnostics.Stopwatch;
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
            Stopwatch w = Stopwatch.StartNew();
            

            path = path.Replace("\\", "/").Replace(@"\", "/");
            tab.path.Set(path);

            if (saveToHistory)
            {
                history.Add(path);
            }

            Stopwatch w1 = Stopwatch.StartNew();
            DeselectAll();
            foreach (var item in entries)
            {
                pool.Despawn(item);
            }
            entries.Clear();
            w1.Stop();
            Debug.Log(" -- cleared in " + w1.ElapsedMilliseconds);

            w1.Restart();
            var folderSortingData = settings.folderSortingData.FirstOrDefault(d => d.path == path);

            IEnumerable<string> folderEnties = Directory.GetDirectories(path).Union(Directory.GetFiles(path));

            if (folderSortingData != null && folderSortingData.isSortingByDate)
            {
                folderEnties = EntryUtils.OrderByModifyDate(folderEnties);
            }
            w1.Stop();
            Debug.Log(" -- enties handled in " + w1.ElapsedMilliseconds);

            w1.Restart();
            foreach (string entryPath in folderEnties)
            {
                SpawnItem(entryPath);
            }
            w1.Stop();
            Debug.Log(" -- spawned in " + w1.ElapsedMilliseconds);

            fileWatcher.Path = CurrentPath;

            onPathChanged?.Invoke();

            w.Stop();
            Debug.Log("Showed in " + w.ElapsedMilliseconds + "ms");
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
        private void SpawnItem(string entryPath)
        {
            if (Path.GetExtension(entryPath) == ".meta")
            {
                string metaTargetFile = entryPath.Substring(0, entryPath.Length - ".meta".Length);
                if (EntryUtils.Exists(metaTargetFile)) return;
            }

            var item = pool.Spawn(entryPath);
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