using UnityEngine;

namespace InApp.UI
{
    public class ProjectFolderHint : IPathBarHint
    {
        private ProjectFolderData data;
        private IconsSO icons;

        public ProjectFolderHint(ProjectFolderData data, IconsSO icons)
        {
            this.data = data;
            this.icons = icons;
        }

        public string GetDisplayText(string input)
        {
            return data.displayText;
        }

        public string GetFullPath()
        {
            return data.path;
        }

        public Sprite GetIcon()
        {
            return icons.browser;
        }

        public int GetMatchesCount(string input)
        {
            int matches = 0;

            input = input.ToLower();
            string[] inputWords = input.Split();
            string displayTextLowered = data.displayText.ToLower();

            foreach (string word in inputWords)
            {
                if (string.IsNullOrWhiteSpace(word)) continue;

                int maxMatches = 0;
                int currentMatch = 0;
                int wordIndex = 0;
                for (int i = 0; i < data.displayText.Length; i++)
                {
                    if (word[wordIndex] == displayTextLowered[i])
                    {
                        wordIndex++;
                        currentMatch++;

                        if (wordIndex >= word.Length)
                        {
                            maxMatches = Mathf.Max(maxMatches, currentMatch);
                            break;
                        }
                    }
                    else
                    {
                        wordIndex = 0;
                        maxMatches = Mathf.Max(maxMatches, currentMatch);
                        currentMatch = 0;
                    }
                }
                matches += maxMatches;
            }
            
            return matches;
        }

        public string GetTypeText()
        {
            return "Папка в проекте";
        }
    }
}