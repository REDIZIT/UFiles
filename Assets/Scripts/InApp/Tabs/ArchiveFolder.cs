using InApp.UI;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Zenject;

namespace InApp
{
    public class ArchiveFolder : Folder
    {
        public string ArchiveFilePath { get; private set; }
        public string ExtractedFolderPath { get; private set; }

        private string ExtractedInArchivePath => ExtractedFolderPath + inArchiveLocalPath;

        private string inArchiveLocalPath;
        private ArchivePath archivePath;
        private ArchiveViewer viewer;

        public ArchiveFolder(string archiveFilePath)
        {
            this.ArchiveFilePath = archiveFilePath;
        }
        private ArchiveFolder(string archiveFilePath, string inArchiveLocalPath) : this(archiveFilePath)
        {
            this.ArchiveFilePath = archiveFilePath;
            this.inArchiveLocalPath = inArchiveLocalPath;
        }

        [Inject]
        private void Construct(ArchiveViewer viewer)
        {
            Debug.Log("Construct, viewer = " + viewer);
            this.viewer = viewer;
            archivePath = viewer.OpenArchive(ArchiveFilePath);
            ExtractedFolderPath = archivePath.GetFullPath();
        }

        public override string GetFullPath()
        {
            return ExtractedInArchivePath;
        }
        public override string GetShortName()
        {
            if (string.IsNullOrEmpty(inArchiveLocalPath))
            {
                return new FileInfo(ArchiveFilePath).Name;
            }
            else
            {
                return new DirectoryInfo(ExtractedInArchivePath).Name;
            }
        }
        public override IEnumerable<Entry> GetEntries()
        {
            return EnumerateEntries(ExtractedInArchivePath);
        }
        public override Folder Open(Entry entry)
        {
            string newInArchiveLocalPath = inArchiveLocalPath + "/" + entry.name;
            return new ArchiveFolder(ArchiveFilePath, newInArchiveLocalPath)
            {
                ExtractedFolderPath = ExtractedFolderPath
            };
        }
        public void Close()
        {
            Debug.Log("Is viewer null? " + (viewer == null));
            viewer.CloseArchive(archivePath);
        }
    }
}