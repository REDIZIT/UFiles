using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

namespace InApp
{
    public class FolderMoveOperation : FileOperation
    {
        private List<string> movedEntries = new List<string>();

        private string sourceFolderPath, destFolderPath;
        private Type type;

        public enum Type
        {
            WholeFolder,
            OnlyContent,
            ContentWithFolderRemove
        }

        public FolderMoveOperation(string sourceFolderPath, string destFolderPath, Type type)
        {
            this.sourceFolderPath = sourceFolderPath;
            this.destFolderPath = destFolderPath;
            this.type = type;
        }

        public override Task Run()
        {
            movedEntries.Clear();

            if (type == Type.WholeFolder)
            {
                Directory.Move(sourceFolderPath, destFolderPath);
            }
            else if (type == Type.OnlyContent || type == Type.ContentWithFolderRemove)
            {
                foreach (string entryPath in Directory.EnumerateFileSystemEntries(sourceFolderPath))
                {
                    string entryName = EntryUtils.GetName(entryPath);
                    string dest = destFolderPath + "/" + entryName;
                    EntryUtils.Move(entryPath, dest);

                    movedEntries.Add(dest);
                }

                if (type == Type.ContentWithFolderRemove)
                {
                    Directory.Delete(sourceFolderPath, true);
                }
            }

            return Task.CompletedTask;
        }

        public override Task Undo()
        {
            if (type == Type.WholeFolder)
            {
                Directory.Move(destFolderPath, sourceFolderPath);
            }
            else if (type == Type.OnlyContent || type == Type.ContentWithFolderRemove)
            {
                if (type == Type.ContentWithFolderRemove)
                {
                    Directory.CreateDirectory(sourceFolderPath);
                }

                foreach (string entryPath in movedEntries)
                {
                    string entryName = EntryUtils.GetName(entryPath);
                    string dest = sourceFolderPath + "/" + entryName;
                    EntryUtils.Move(entryPath, dest);
                }
            }
            return Task.CompletedTask;
        }
    }
}