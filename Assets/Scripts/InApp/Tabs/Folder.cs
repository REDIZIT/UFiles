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
                DirectoryInfo info = new DirectoryInfo(folder);
                int subEntriesCount = TryGetSubElementsCount(folder);

                yield return new Entry()
                {
                    name = info.Name,
                    size = subEntriesCount,
                    isFolder = true,
                    lastWriteTime = info.LastWriteTime
                };
            }
            foreach (string file in Directory.EnumerateFiles(path))
            {
                FileInfo info = new FileInfo(file);

                yield return new Entry()
                {
                    name = info.Name,
                    size = info.Length,
                    isFolder = false,
                    lastWriteTime = info.LastWriteTime
                };
            }
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