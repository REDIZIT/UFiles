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
        private List<EntryUIItem> selectedEntries = new();
        private EntryUIItem.Pool pool;


        [Inject]
        private void Construct(EntryUIItem.Pool pool)
        {
            this.pool = pool;
        }

        private void Start()
        {
            Show("C:\\Users\\redizit\\UFiles\\Assets");
        }
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape) && selectedEntries.Count > 0)
            {
                DeselectAll();
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

            onPathChanged?.Invoke();
        }
        public void Refresh()
        {
            Show(CurrentPath);
        }
        public void OnItemClicked(EntryUIItem item)
        {
            bool isShift = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
            bool isControl = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);

            if (isShift == false && isControl == false)
            {
                DeselectAll();
            }

            if (isShift && selectedEntries.Count > 0)
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
    }

}