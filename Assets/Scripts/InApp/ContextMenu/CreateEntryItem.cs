using System.IO;
using UnityEngine;
using Zenject;

namespace InApp.UI
{
    public class CreateEntryItem : ContextItem
    {
        [Inject] private CreateRenameWindow window;
        private bool createDirectory;

        public CreateEntryItem()
        {
            text = "Создать";

            children.Add(new CreateConcreteEntryItem(true));
            children.Add(new CreateConcreteEntryItem(false));
        }

        public override Texture2D GetIcon()
        {
            return icons.context.create;
        }

        public override void OnClick(ContextItemEnvironment env)
        {
            window.Show(env, createDirectory, "", null, filepath =>
            {
                string path = env.currentFolder + "/" + filepath;
                if (createDirectory)
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
    public class CreateConcreteEntryItem : ContextItem
    {
        private bool isFolder;

        public CreateConcreteEntryItem(bool isFolder)
        {
            this.isFolder = isFolder;
            text = isFolder ? "Папку" : "Файл";
        }

        public override Texture2D GetIcon()
        {
            return isFolder ? icons.folderEmpty : icons.defaultFile;
        }
    }
}