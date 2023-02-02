using UnityEngine;

namespace InApp.UI
{
    public class LocalFolderHint : IPathBarHint
    {
        private string path, displayText;
        private IconsSO icons;

        public LocalFolderHint(string path, string displayText, IconsSO icons)
        {
            this.path = path;
            this.displayText = displayText;
            this.icons = icons;
        }

        public string GetDisplayText(string input)
        {
            return displayText;
        }
        public string GetFullPath()
        {
            return path;
        }
        public string GetTypeText()
        {
            return "Системная папка";
        }
        public int GetMatchesCount(string input)
        {
            int matches = 0;

            input = input.ToLower();
            string displayTextLowered = displayText.ToLower();

            for (int i = 0; i < Mathf.Min(input.Length, displayText.Length); i++)
            {
                if (input[i] == displayTextLowered[i])
                {
                    matches++;
                }
                else
                {
                    return matches;
                }
            }
            return matches;
        }
        public Sprite GetIcon()
        {
            return icons.pathBar.userFolder;
        }
    }
}