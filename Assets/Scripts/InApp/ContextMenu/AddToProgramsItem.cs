using System;
using System.IO;
using UnityEngine;
using Zenject;

namespace InApp.UI
{
    public class AddToProgramsItem : ContextItem
    {
        [Inject] private Bridge bridge;

        private string filePath;
        private string programsFolder;

        public AddToProgramsItem(string filePath)
        {
            this.filePath = filePath;

            if (IsInPrograms(out _))
            {
                text = "Убрать из программ";
            }
            else
            {
                text = "Добавить в программы";
            }
        }
        public override Texture2D GetIcon()
        {
            return icons.context.list.texture;
        }
        public override void OnClick(ContextItemEnvironment env)
        {
            base.OnClick(env);
            if (IsInPrograms(out string programFilePath))
            {
                File.Delete(programFilePath);
            }
            else
            {
                appShortcutToDesktop(programFilePath, filePath);
            }
        }

        private bool IsInPrograms(out string programFilePath)
        {
            string userFolder = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            programsFolder = userFolder + "/AppData/Roaming/Microsoft/Windows/Start Menu/Programs";

            string fileName = Path.GetFileName(filePath);
            programFilePath = programsFolder + "/" + fileName;
            return File.Exists(programFilePath + ".lnk");
        }
        private void appShortcutToDesktop(string linkPath, string targetAppPath)
        {
            bridge.Enqueue(new FileShortcutCommand(linkPath, targetAppPath));
        }
    }
}