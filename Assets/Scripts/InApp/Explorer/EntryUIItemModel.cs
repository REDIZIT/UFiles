using System;
using System.IO;

namespace InApp.UI
{
    public class EntryUIItemModel
    {
        public float Height => 32 + (isExpanded ? 32 : 0);

        public Entry entry;
        public EntryUIItemModel metaModel;

        public DateTime lastWriteTime;

        public bool isExpanded;
        public bool isAdditionalInformationRead;
        public bool isSelected
        {
            get { return _isSelected; }
            set { _isSelected = value; onSelectChanged?.Invoke(); }
        }
        private bool _isSelected;

        public Action onSelectChanged;

        public EntryUIItemModel(Entry entry, Folder folder)
        {
            this.entry = entry;

            if (folder.TryGetEntry(entry.name + ".meta", out Entry metaEntry))
            {
                metaModel = new EntryUIItemModel(metaEntry, folder);
            }
        }

        public void GetAdditionalInformation(Folder folder)
        {
            string path = entry.GetFullPathFor(folder);
            if (EntryUtils.TryGetType(path, out EntryType type))
            {
                if (type == EntryType.File)
                {
                    FileInfo info = new FileInfo(path);
                    lastWriteTime = info.LastWriteTime;
                    entry.size = info.Length;
                }
                else
                {
                    DirectoryInfo info = new DirectoryInfo(path);
                    lastWriteTime = info.LastWriteTime;
                }
            }

            if (metaModel != null)
            {
                metaModel.GetAdditionalInformation(folder);
            }

            isAdditionalInformationRead = true;
        }
    }
}