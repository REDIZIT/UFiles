using UnityEngine;

namespace InApp.UI
{
    public class LocalFolderHint : BaseHint
    {
        protected string path, displayText;

        public LocalFolderHint(IconsSO icons, string path, string displayText) : base(icons)
        {
            this.path = path;
            this.displayText = displayText;
        }

        public override string GetDisplayText(string input)
        {
            return displayText;
        }
        public override string GetFullPath()
        {
            return path;
        }
        public override string GetTypeText()
        {
            return "Системная папка";
        }
        public override Sprite GetIcon()
        {
            return icons.pathBar.userFolder;
        }
    }
}