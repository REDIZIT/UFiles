using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using UnityEngine.Profiling;
using Zenject;

namespace InApp
{
    public abstract class Folder
    {
        [Inject] protected DiContainer container;

        public abstract string GetFullPath();
        public abstract string GetShortName();
        public abstract IEnumerable<Entry> GetEntries();
        public abstract Folder Open(Entry entry);

        protected IEnumerable<Entry> EnumerateEntries(string path)
        {
            Profiler.BeginSample("Enumerate folders");
            foreach (string folder in Directory.EnumerateDirectories(path))
            {
                yield return GetFolderEntry(folder);
            }
            Profiler.EndSample();
            Profiler.BeginSample("Enumerate files");
            foreach (string file in Directory.EnumerateFiles(path))
            {
                if (file.EndsWith(".meta")) continue;
                yield return GetFileEntry(file);
            }
            Profiler.EndSample();
        }

        public bool TryGetEntry(string entryName, out Entry entry)
        {
            string path = GetFullPath() + "/" + entryName;
            if (File.Exists(path))
            {
                entry = GetFileEntry(path);
                return true;
            }
            entry = default;
            return false;
        }

        private Entry GetFolderEntry(string path)
        {
            //DirectoryInfo info = new DirectoryInfo(path);
            long subEntriesCount = TryGetSubElementsCount(path);

            return new Entry()
            {
                name = Path.GetFileName(path),
                size = subEntriesCount,
                isFolder = true
                //lastWriteTime = info.LastWriteTime,
                //metaEntry = GetMetaEntry(path)
            };
        }
        private Entry GetFileEntry(string path)
        {
            //FileInfo info = new FileInfo(path);

            return new Entry()
            {
                name = Path.GetFileName(path),
                //size = (uint)info.Length,
                isFolder = false
                //lastWriteTime = info.LastWriteTime,
                //metaEntry = GetMetaEntry(path)
            };
        }
        private long TryGetSubElementsCount(string folder)
        {
            try
            {
                return Directory.EnumerateFileSystemEntries(folder).Count();
            }
            catch
            {
                return -1;
            }
        }
    }
}