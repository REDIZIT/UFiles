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

        public float GetMatchesCount(string input)
        {
            float matches = 0;

            input = input.ToLower();
            string[] inputWords = input.Split();
            string displayTextLowered = data.displayText.ToLower();
            int missedChars = 0;

            foreach (string word in inputWords)
            {
                if (string.IsNullOrWhiteSpace(word)) continue;

                int maxMatches = 0;
                int currentMatch = 0;
                int wordIndex = 0;
                bool isStartsWith = true;
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
                        isStartsWith = false;
                        maxMatches = Mathf.Max(maxMatches, currentMatch);
                        currentMatch = 0;
                        missedChars++;
                    }
                }
                // TODO: include overword as missing chars
                matches += maxMatches * 2 + (isStartsWith ? 10 : 0)/* - data.depth*/ - missedChars / 5f;
            }
            return matches;
        }

        public string GetTypeText()
        {
            return "Папка в проекте";
        }
    }
}