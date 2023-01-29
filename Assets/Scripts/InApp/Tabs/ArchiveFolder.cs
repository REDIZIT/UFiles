using InApp.UI;
using System.Collections.Generic;
using System.IO;
using Zenject;

namespace InApp
{
    public class ArchiveFolder : Folder
    {
        public string ArchiveFilePath { get; private set; }

        private string ExtractedInArchivePath => extractedFolderPath + inArchiveLocalPath;

        private string inArchiveLocalPath;
        private string extractedFolderPath;

        public ArchiveFolder(string archiveFilePath)
        {
            this.ArchiveFilePath = archiveFilePath;
        }
        public ArchiveFolder(string archiveFilePath, string inArchiveLocalPath) : this(archiveFilePath)
        {
            this.ArchiveFilePath = archiveFilePath;
            this.inArchiveLocalPath = inArchiveLocalPath;
        }

        [Inject]
        private void Construct(ArchiveViewer viewer)
        {
            ArchivePath path = viewer.OpenArchive(ArchiveFilePath);
            extractedFolderPath = path.GetFullPath();
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
                extractedFolderPath = extractedFolderPath
            };
        }
    }
}