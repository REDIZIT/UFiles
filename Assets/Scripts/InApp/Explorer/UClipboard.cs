using System.Collections.Generic;
using System.IO;

namespace InApp
{
    public class UClipboard
    {
        private List<string> buffer = new();
        private CopyType copyType;
        
        public enum CopyType
        {
            Copy,
            Cut
        }

        public void Copy(IEnumerable<string> items, CopyType copyType)
        {
            buffer.Clear();
            buffer.AddRange(items);
            this.copyType = copyType;
        }
        public void Paste(string targetFolder)
        {
            foreach (string filepath in buffer)
            {
                string filename = Path.GetFileName(filepath);
                string targetFilepath = targetFolder + "/" + filename;

                EntryUtils.Copy(filepath, targetFilepath);

                if (copyType == CopyType.Cut)
                {
                    EntryUtils.Delete(filepath);
                }
            }

            if (copyType == CopyType.Cut)
            {
                buffer.Clear();
            }
        }
    }
}