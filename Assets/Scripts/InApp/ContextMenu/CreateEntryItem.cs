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

            children.Add(new CreateConcreteEntryItem(true));
            children.Add(new CreateConcreteEntryItem(false));
        }

        public override Texture2D GetIcon()
        {
            return icons.context.create;
        }
    }
    public class CreateConcreteEntryItem : ContextItem
    {
        [Inject] private CreateRenameWindow window;

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

        public override void OnClick(ContextItemEnvironment env)
        {
            window.Show(env, isFolder, "", null, filepath =>
            {
                string path = env.currentFolder + "/" + filepath;
                if (isFolder)
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