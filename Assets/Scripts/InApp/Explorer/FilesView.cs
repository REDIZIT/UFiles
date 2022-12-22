using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using Zenject;

namespace InApp.UI
{
    public class FilesView : MonoBehaviour
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
        private EntryUIItem.Pool pool;

        [Inject]
        private void Construct(EntryUIItem.Pool pool)
        {
            this.pool = pool;
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
                    DeleteSelected();
                }
            }

            if (hasFileSystemChanges)
            {
                Show(CurrentPath);
                hasFileSystemChanges = false;
            }
        }

        public void Show(string path)
        {
            path = path.Replace("\\", "/").Replace(@"\", "/");
            CurrentPath = path;

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
                        entries[i].SetSelection(true);
                        selectedEntries.Add(entries[i]);
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
        private void DeleteSelected()
        {
            foreach (var entry in selectedEntries)
            {
                if (Directory.Exists(entry.Path)) Directory.Delete(entry.Path);
                else File.Delete(entry.Path);

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