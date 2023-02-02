using System.Collections.Generic;
using System.IO;
using System.Linq;
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
            foreach (string folder in Directory.EnumerateDirectories(path))
            {
                yield return GetFolderEntry(folder);
            }
            foreach (string file in Directory.EnumerateFiles(path))
            {
                if (file.EndsWith(".meta")) continue;
                yield return GetFileEntry(file);
            }
        }
        private Entry GetFolderEntry(string path)
        {
            DirectoryInfo info = new DirectoryInfo(path);
            //int subEntriesCount = TryGetSubElementsCount(path);

            return new Entry()
            {
                name = info.Name,
                //size = subEntriesCount,
                isFolder = true,
                lastWriteTime = info.LastWriteTime,
                metaEntry = GetMetaEntry(path)
            };
        }
        private Entry GetFileEntry(string path)
        {
            FileInfo info = new FileInfo(path);

            return new Entry()
            {
                name = info.Name,
                size = info.Length,
                isFolder = false,
                lastWriteTime = info.LastWriteTime,
                metaEntry = GetMetaEntry(path)
            };
        }
        private Entry GetMetaEntry(string assetFilePath)
        {
            string metaPath = assetFilePath + ".meta";
            if (File.Exists(metaPath))
            {
                return GetFileEntry(metaPath);
            }
            return null;
        }
        private int TryGetSubElementsCount(string folder)
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