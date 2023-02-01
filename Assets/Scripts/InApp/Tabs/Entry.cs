using System;

namespace InApp
{
    public class Entry
    {
        public string name;
        public long size;
        public DateTime lastWriteTime;
        public bool isFolder;

        public Entry metaEntry;

        public string GetFullPathFor(Folder folder)
        {
            return folder.GetFullPath() + "/" + name;
        }
    }
}