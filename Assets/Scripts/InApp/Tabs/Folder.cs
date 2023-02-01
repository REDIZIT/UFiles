using System.Collections.Generic;
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
        private Entry GetFolderEntry(string path)
        {
            //DirectoryInfo info = new DirectoryInfo(path);
            //uint subEntriesCount = TryGetSubElementsCount(path);

            return new Entry()
            {
                name = Path.GetFileName(path),
                //size = subEntriesCount,
                isFolder = true,
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
                isFolder = false,
                //lastWriteTime = info.LastWriteTime,
                //metaEntry = GetMetaEntry(path)
            };
        }
        //private Entry GetMetaEntry(string assetFilePath)
        //{
        //    string metaPath = assetFilePath + ".meta";
        //    if (File.Exists(metaPath))
        //    {
        //        return GetFileEntry(metaPath);
        //    }
        //    return null;
        //}
        private uint TryGetSubElementsCount(string folder)
        {
            return 0;
            //try
            //{
            //    return Directory.EnumerateFileSystemEntries(folder).Count();
            //}
            //catch
            //{
            //    return -1;
            //}
        }
    }
}