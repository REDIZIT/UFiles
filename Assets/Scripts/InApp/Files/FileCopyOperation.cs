using InApp.UI;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace InApp
{
    public class FileCopyOperation : FileOperation
    {
        private List<string> entriesToCopy = new List<string>();
        private string targetDirectory;
        private List<string> copiedEntries = new List<string>();

        [Inject] private CopyConflictWindow copyConflict;

        public FileCopyOperation(List<string> entriesToCopy, string targetDirectory)
        {
            this.entriesToCopy.AddRange(entriesToCopy);
            this.targetDirectory = targetDirectory;
        }

        public override async Task Run()
        {
            copiedEntries.Clear();

            foreach (string entryPath in entriesToCopy)
            {
                string entryName = Path.GetFileName(entryPath);
                string targetEntryPath = targetDirectory + "/" + entryName;

                CopyConflictWindow.Answer answer = CopyConflictWindow.Answer.Overwrite;
                if (File.Exists(targetEntryPath))
                {
                    answer = await copyConflict.ShowDialog(new CopyConflictWindow.Model()
                    {
                        filepath = targetEntryPath,
                        sourceFolder = entryName,
                        targetFolder = targetEntryPath
                    });
                }

                if (answer == CopyConflictWindow.Answer.Rename)
                {
                    targetEntryPath = EntryUtils.FindClosestFreeName(targetEntryPath);
                }

                if (answer != CopyConflictWindow.Answer.Cancel)
                {
                    EntryUtils.Copy(entryPath, targetEntryPath, true);
                    copiedEntries.Add(targetEntryPath);
                }
            }
        }
        public override Task Undo()
        {
            foreach (string entryPath in copiedEntries)
            {
                EntryUtils.Delete(entryPath);
                EntryUtils.Delete(entryPath + ".meta");
            }
            return Task.CompletedTask;
        }

        public override string ToString()
        {
            return entriesToCopy[0];
        }
    }
}