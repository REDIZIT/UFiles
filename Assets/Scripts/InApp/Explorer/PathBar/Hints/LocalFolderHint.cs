using UnityEngine;

namespace InApp.UI
{
    public class LocalFolderHint : IPathBarHint
    {
        private string path, displayText;

        public LocalFolderHint(string path, string displayText)
        {
            this.path = path;
            this.displayText = displayText;
        }

        public string GetDisplayText(string input)
        {
            return displayText;
        }
        public string GetFullPath()
        {
            return path;
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
    }
}