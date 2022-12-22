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

        private List<EntryUIItem> entries = new();
        private HashSet<EntryUIItem> selectedEntries = new();
        private FileSystemWatcher fileWatcher = new();
        private bool hasFileSystemChanges;
        private List<string> history = new();
        private int historyIndex = -1;

        private EntryUIItem.Pool pool;
        private ContextMenuCreator context;

        [Inject]
        private void Construct(EntryUIItem.Pool pool, ContextMenuCreator context)
        {
            this.pool = pool;
            this.context = context;

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
                    DeleteSelected(Input.GetKey(KeyCode.LeftShift));
                }
            }

            if (hasFileSystemChanges)
            {
                Show(CurrentPath);
                hasFileSystemChanges = false;
            }

            if (Input.GetMouseButtonDown(3) && historyIndex > 0)
            {
                historyIndex--;
                Show(history[historyIndex], true);
            }
            if (Input.GetMouseButtonDown(4) && historyIndex < history.Count - 1)
            {
                historyIndex++;
                Show(history[historyIndex], true);
            }
        }

        public void Show(string path, bool fromHistrory = false)
        {
            path = path.Replace("\\", "/").Replace(@"\", "/");
            CurrentPath = path;

            if (fromHistrory == false)
            {
                if (history.Count > 1)
                {
                    history.RemoveRange(historyIndex + 1, history.Count - historyIndex - 1);
                }
                history.Add(path);
                historyIndex++;
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
                    new CreateEntryItem()
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
            if (Path.GetExtension(entryPath) == ".meta") return;

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
                    if (Directory.Exists(entry.Path)) Directory.Delete(entry.Path);
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