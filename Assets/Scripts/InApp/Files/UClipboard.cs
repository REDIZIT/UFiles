using System.Collections.Generic;
using System.IO;
using Zenject;

namespace InApp
{
    public class UClipboard
    {
        [Inject] private FileOperator fileOperator;

        private List<string> buffer = new List<string>();
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

//#if UNITY_EDITOR == false
//            Program.SetClipboardData(buffer);
//#endif
        }
        public void Paste(string targetFolder)
        {
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