using InApp.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Zenject;

namespace InApp
{
    public interface IPath
    {
        void Set(string path);
        string GetDisplayName();
        string GetFullPath();
    }
    public abstract class Folder
    {
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
    public class LocalFolder : Folder
    {
        private string path;

        public LocalFolder(string localFolderPath)
        {
            path = localFolderPath;
        }

        public override string GetFullPath()
        {
            return path;
        }
        public override string GetShortName()
        {
            return new DirectoryInfo(path).Name;
        }
        public override IEnumerable<Entry> GetEntries()
        {
            return EnumerateEntries(path);
        }
        public override Folder Open(Entry entry)
        {
            string entryFullPath = path + "/" + entry.name;
            if (ArchiveViewer.IsArchive(entryFullPath))
            {
                return new ArchiveFolder(entryFullPath);
            }
            else
            {
                return new LocalFolder(entryFullPath);
            }
        }
    }
    public class ArchiveFolder : Folder
    {
        private string ExtractedInArchivePath => extractedFolderPath + inArchiveLocalPath;

        private string archiveFilePath;
        private string inArchiveLocalPath;
        private string extractedFolderPath;

        public ArchiveFolder(string archiveFilePath)
        {
            this.archiveFilePath = archiveFilePath;
        }
        public ArchiveFolder(string archiveFilePath, string inArchiveLocalPath) : this(archiveFilePath)
        {
            this.archiveFilePath = archiveFilePath;
            this.inArchiveLocalPath = inArchiveLocalPath;
        }

        [Inject]
        private void Construct(ArchiveViewer viewer)
        {
            ArchivePath path = viewer.OpenArchive(archiveFilePath);
            extractedFolderPath = path.GetFullPath();
        }

        public override string GetFullPath()
        {
            return ExtractedInArchivePath;
        }
        public override string GetShortName()
        {
            return Path.GetDirectoryName(ExtractedInArchivePath);
        }
        public override IEnumerable<Entry> GetEntries()
        {
            return EnumerateEntries(ExtractedInArchivePath);
        }
        public override Folder Open(Entry entry)
        {
            string newInArchiveLocalPath = inArchiveLocalPath + "/" + entry.name;
            return new ArchiveFolder(archiveFilePath, newInArchiveLocalPath)
            {
                extractedFolderPath = extractedFolderPath
            };
        }
    }
    public class Entry
    {
        public string name;
        public long size;
        public DateTime lastWriteTime;
        public bool isFolder;

        public string GetFullPathFor(Folder folder)
        {
            return folder.GetFullPath() + "/" + name;
        }
    }
}