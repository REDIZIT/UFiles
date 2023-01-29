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
            if (ArchiveViewer.IsArchive(entryFullPath))
            {
                ArchiveFolder folder = new ArchiveFolder(entryFullPath);
                container.Inject(folder);
                return folder;
            }
            else
            {
                LocalFolder folder = new LocalFolder(entryFullPath);
                container.Inject(folder);
                return folder;
            }
        }
    }
}