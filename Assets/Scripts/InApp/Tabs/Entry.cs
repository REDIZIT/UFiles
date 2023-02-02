namespace InApp
{
    public struct Entry
    {
        public string name;
        public bool isFolder;
        public long size;
        public string GetFullPathFor(Folder folder)
        {
            return folder.GetFullPath() + "/" + name;
        }
    }
}