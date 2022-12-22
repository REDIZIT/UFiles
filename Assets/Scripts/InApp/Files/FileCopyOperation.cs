using System.Collections.Generic;
using System.IO;

namespace InApp
{
    public class FileCopyOperation : FileOperation
    {
        private List<string> entriesToCopy = new();
        private string targetDirectory;
        private List<string> copiedEntries = new();

        public FileCopyOperation(List<string> entriesToCopy, string targetDirectory)
        {
            this.entriesToCopy.AddRange(entriesToCopy);
            this.targetDirectory = targetDirectory;
        }

        public override void Run()
        {
            copiedEntries.Clear();

            foreach (string entryPath in entriesToCopy)
            {
                string entryName = Path.GetFileName(entryPath);
                string targetEntryPath = targetDirectory + "/" + entryName;

                copiedEntries.Add(targetEntryPath);
                EntryUtils.Copy(entryPath, targetEntryPath);
            }
        }
        public override void Undo()
        {
            foreach (string entryPath in copiedEntries)
            {
                EntryUtils.Delete(entryPath);
                EntryUtils.Delete(entryPath + ".meta");
            }
        }

        public override string ToString()
        {
            return entriesToCopy[0];
        }
    }
}