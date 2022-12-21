using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Zenject;

namespace InApp.UI
{
    public class FilesView : MonoBehaviour
    {
        public string CurrentPath { get; private set; }

        public Action onPathChanged;

        [SerializeField] private Transform content;

        private List<EntryUIItem> entries = new List<EntryUIItem>();
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
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Start();
            }
        }

        public void Show(string path)
        {
            path = path.Replace("\\", "/").Replace(@"\", "/");
            CurrentPath = path;

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
        private void SpawnItem(string entryPath)
        {
            if (Path.GetExtension(entryPath) == ".meta") return;

            var item = pool.Spawn(entryPath);
            item.transform.parent = content;
            item.transform.SetAsLastSibling();

            entries.Add(item);
        }

        private static bool FileOrDirectoryExists(string name)
        {
            return Directory.Exists(name) || File.Exists(name);
        }
    }

}