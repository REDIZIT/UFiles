using System.IO;
using UnityEngine;
using Zenject;

namespace InApp.UI
{
    public class CreateEntryItem : ContextItem
    {
        public CreateEntryItem()
        {
            text = "Создать";

            children.Add(new CreateConcreteEntryItem(EntryType.Directory));
            children.Add(new CreateConcreteEntryItem(EntryType.File));
        }

        public override Texture2D GetIcon()
        {
            return icons.context.create.texture;
        }
    }
    public class CreateConcreteEntryItem : ContextItem
    {
        [Inject] private CreateRenameWindow window;

        private EntryType entryType;

        public CreateConcreteEntryItem(EntryType entryType)
        {
            this.entryType = entryType;
            text = entryType == EntryType.Directory ? "Папку" : "Файл";
        }

        public override Texture2D GetIcon()
        {
            return entryType == EntryType.Directory ? icons.folderEmpty.texture : icons.defaultFile.texture;
        }

        public override void OnClick(ContextItemEnvironment env)
        {
            string title = "Создать " + (entryType == EntryType.Directory ? "папку" : "файл");
            window.Show(new CreateRenameWindow.Model()
            {
                env = env,
                title = title,
                entryType = entryType,
                filename = "",
                onApply = filepath =>
                {
                    string path = env.currentFolder + "/" + filepath;
                    if (entryType == EntryType.Directory)
                    {
                        Directory.CreateDirectory(path);
                    }
                    else
                    {
                        File.Create(path).Dispose();
                    }
                }
            });
        }
    }
}