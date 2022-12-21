using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace InApp.UI
{
    public class ContextItem
    {
        public string text;
        public List<ContextItem> children = new();

        public virtual void OnClick(ContextItemEnvironment env) { }
    }
    public class ContextItemEnvironment
    {
        public string currentFolder;
    }

    public class CreateFileItem : ContextItem
    {
        private CreateRenameWindow window;
        private FilesView files;

        public CreateFileItem(CreateRenameWindow window, FilesView files)
        {
            this.window = window;
            this.files = files;
        }

        public override void OnClick(ContextItemEnvironment env)
        {
            window.Show(env, "123.txt", null, filepath =>
            {
                File.Create(env.currentFolder + "/" + filepath).Dispose();
                files.Refresh();
            });
        }
    }
}