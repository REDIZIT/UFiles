using InApp.UI;
using System.Collections.Generic;
using System.IO;

namespace InApp
{
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
            Folder folder;
            if (ArchiveViewer.IsArchive(entryFullPath))
            {
                folder = new ArchiveFolder(entryFullPath);
            }
            else
            {
                folder = new LocalFolder(entryFullPath);
            }

            container.Inject(folder);
            return folder;
        }
    }
}