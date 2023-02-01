namespace InApp.UI
{
    public class EntryUIItemModel
    {
        public float Height => 32 + (isExpanded ? 32 : 0);

        public Entry entry;
        public EntryUIItemModel metaModel;

        public bool isExpanded;

        public EntryUIItemModel(Entry entry, Folder folder)
        {
            this.entry = entry;

            if (folder.TryGetEntry(entry.name + ".meta", out Entry metaEntry))
            {
                metaModel = new EntryUIItemModel(metaEntry, folder);
            }
        }
    }
}