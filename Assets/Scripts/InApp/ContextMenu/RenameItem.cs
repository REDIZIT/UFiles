using System.IO;
using UnityEngine;
using Zenject;

namespace InApp.UI
{
    public class RenameFileItem : ContextItem
    {
        [Inject] private CreateRenameWindow renameWindow;

        public RenameFileItem()
        {
            text = "Переименовать";
        }
        public override Texture2D GetIcon()
        {
            return icons.context.rename.texture;
        }
        public override void OnClick(ContextItemEnvironment env)
        {
            var type = Directory.Exists(env.selectedEntryName) ? EntryType.Directory : EntryType.File;
            string name, parentFolder;

            if (type == EntryType.Directory)
            {
                var info = new DirectoryInfo(env.selectedEntryName);
                name = info.Name;
                parentFolder = info.Parent.FullName;
            }
            else
            {
                var info = new FileInfo(env.selectedEntryName);
                name = info.Name;
                parentFolder = info.Directory.FullName;
            }

            renameWindow.Show(new CreateRenameWindow.Model()
            {
                env = env,
                title = "Переименовать",
                entryType = type, 
                filename = name,
                onApply = newName =>
                {
                    string sourcePath = parentFolder + "/" + name;
                    string targetPath = parentFolder + "/" + newName;

                    EntryUtils.Move(sourcePath, targetPath);
                }
            });
        }
    }
}