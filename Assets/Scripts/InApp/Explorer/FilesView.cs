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
        public string CurrentPath { get; private set; }
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

        [Inject]
        private void Construct(EntryUIItem.Pool pool, ContextMenuCreator context, FileOperator fileOperator)
        {
            this.pool = pool;
            this.context = context;
            this.fileOperator = fileOperator;

            fileOperator.onAnyOperationApplied += () => FilesChanged(null, null);

            fileWatcher.Filter = "*.*";
            fileWatcher.NotifyFilter = NotifyFilters.FileName | NotifyFilters.DirectoryName;
            fileWatcher.Created += FilesChanged;
            fileWatcher.Deleted += FilesChanged;
            fileWatcher.Renamed += FilesChanged;
            fileWatcher.EnableRaisingEvents = true;
        }

        

        private void Start()
        {
            Show("C:\\Users\\redizit\\UFiles\\Assets");
        }
        private void Update()
        {
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

            if (hasFileSystemChanges)
            {
                Show(CurrentPath, false);
                hasFileSystemChanges = false;
            }

            if (Input.GetMouseButtonDown(3) && history.TryUndo(out string directory))
            {
                Show(directory, false);
            }
            if (Input.GetMouseButtonDown(4) && history.TryRedo(out directory))
            {
                Show(directory, false);
            }

            if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
            {
                if (Input.GetKeyDown(KeyCode.Z)) fileOperator.Undo();
                if (Input.GetKeyDown(KeyCode.Y)) fileOperator.Redo();
            }
        }

        public void Show(string path, bool saveToHistory = true)
        {
            path = path.Replace("\\", "/").Replace(@"\", "/");
            CurrentPath = path;

            if (saveToHistory)
            {
                history.Add(path);
            }
            

            DeselectAll();
            foreach (var item in entries)
            {
                pool.Despawn(item);
            }
            entries.Clear();
            

            foreach (string entryPath in Directory.GetDirectories(path))
            {
                SpawnItem(entryPath);
            }
            foreach (string entryPath in Directory.GetFiles(path))
            {
                SpawnItem(entryPath);
            }

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
            selectedEntries.Clear();
        }
        private void FilesChanged(object sender, FileSystemEventArgs e)
        {
            hasFileSystemChanges = true;
        }
    }

}