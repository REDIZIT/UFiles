using System.IO;

namespace InApp.UI
{
    public class CreateEntryItem : ContextItem
    {
        private CreateRenameWindow window;
        private bool createDirectory;

        public CreateEntryItem(CreateRenameWindow window, bool createDirectory)
        {
            this.window = window;
            this.createDirectory = createDirectory;
            text = "Создать " + (createDirectory ? "папку" : "файл");
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
}