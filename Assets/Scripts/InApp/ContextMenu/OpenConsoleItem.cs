using System.Diagnostics;
using UnityEngine;

namespace InApp.UI
{
    public class OpenConsoleItem : ContextItem
    {
        private string folderPath;

        public OpenConsoleItem(string folderPath)
        {
            this.folderPath = folderPath;
            text = "Открыть cmd здесь";
        }

        public override Texture2D GetIcon()
        {
            return icons.context.cmd.texture;
        }

        public override void OnClick(ContextItemEnvironment env)
        {
            ProcessStartInfo info = new ProcessStartInfo()
            {
                WorkingDirectory = folderPath,
                FileName = "cmd.exe"
            };
            Process.Start(info);
        }
    }
}