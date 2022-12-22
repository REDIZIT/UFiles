using System.Collections.Generic;
using System.IO;
using Zenject;

namespace InApp
{
    public class UClipboard
    {
        [Inject] private FileOperator fileOperator;

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
            //foreach (string filepath in buffer)
            //{
            //    string filename = Path.GetFileName(filepath);
            //    string targetFilepath = targetFolder + "/" + filename;

            //    EntryUtils.Copy(filepath, targetFilepath);
            //}
            fileOperator.Run(new FileCopyOperation(buffer, targetFolder));
            
            if (copyType == CopyType.Cut)
            {
                foreach (string filepath in buffer)
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