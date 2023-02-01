namespace InApp.UI
{
    public class EntryUIItemModel
    {
        public Entry entry;

        public EntryUIItemModel(Entry entry)
        {
            this.entry = entry;

            //if (entry.metaEntry != null)
            //{
            //    metaModel = new EntryUIItemModel(entry.metaEntry);
            //}
        }
    }
}