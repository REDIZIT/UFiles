using UnityEngine;

namespace InApp.UI
{
    public class SubFolderHint : IPathBarHint
    {
        private string parentFolder, subFolder;
        private string targetFolder;
        private IconsSO icons;

        public SubFolderHint(string parentFolder, string subFolder, IconsSO icons)
        {
            this.parentFolder = parentFolder;
            this.subFolder = subFolder;
            this.icons = icons;

            targetFolder = parentFolder + subFolder;
        }

        public string GetDisplayText(string input)
        {
            string inputLowered = input.ToLower();
            string targetLowered = targetFolder.ToLower();

            int boldEndIndex = 0;
            for (int i = parentFolder.Length; i < Mathf.Min(inputLowered.Length, targetLowered.Length); i++)
            {
                if (targetLowered[i] == inputLowered[i])
                {
                    boldEndIndex++;
                }
                else
                {
                    break;
                }
            }

            string boldText = targetFolder.Substring(parentFolder.Length, boldEndIndex);
            string unboldedText = targetFolder.Substring(parentFolder.Length + boldEndIndex);

            return "<color=#888>" + parentFolder + "</color>" + boldText + "<color=#888>" + unboldedText + "</color>";
        }
        public string GetFullPath()
        {
            return parentFolder + subFolder;
        }
        public string GetTypeText()
        {
            return "Подпапка";
        }
        public float GetMatchesCount(string input)
        {
            int matches = 0;

            input = input.ToLower();
            string targetFolderLowered = targetFolder.ToLower();

            if (input.EndsWith("/")) return 1;

            for (int i = parentFolder.Length; i < Mathf.Min(input.Length, targetFolder.Length); i++)
            {
                if (input[i] == targetFolderLowered[i])
                {
                    matches++;
                }
                else
                {
                    matches--;
                }
                if (matches < 0) return 0;
            }
            return matches - Mathf.Max(0, input.Length - targetFolder.Length);
        }
        public Sprite GetIcon()
        {
            return icons.pathBar.subFolder;
        }
    }
}