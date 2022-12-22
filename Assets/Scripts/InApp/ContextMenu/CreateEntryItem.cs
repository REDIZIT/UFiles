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
            return icons.context.create;
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
            return entryType == EntryType.Directory ? icons.folderEmpty : icons.defaultFile;
        }

        public override void OnClick(ContextItemEnvironment env)
        {
            string title = "Создать " + (entryType == EntryType.Directory ? "папку" : "файл");
            window.Show(env, title, entryType, "", filepath =>
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
            });
        }
    }
}