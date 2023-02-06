using System.IO;
using UnityEngine;

namespace InApp.UI
{
    public class ProjectHint : BaseHint
    {
        private Project project;

        public ProjectHint(IconsSO icons, Project project) : base(icons)
        {
            this.project = project;
        }

        public override string GetDisplayText(string input)
        {
            return Path.GetFileName(project.mainFolder);
        }

        public override string GetFullPath()
        {
            return project.mainFolder + "/" + project.folderToIndex;
        }

        public override Sprite GetIcon()
        {
            return icons.pathBar.project;
        }

        public override string GetTypeText()
        {
            return "Проект";
        }
    }
}