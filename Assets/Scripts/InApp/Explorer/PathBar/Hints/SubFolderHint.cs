using UnityEngine;

namespace InApp.UI
{
    public class SubFolderHint : IPathBarHint
    {
        private string parentFolder, subFolder;
        private string targetFolder;

        public SubFolderHint(string parentFolder, string subFolder)
        {
            this.parentFolder = parentFolder;
            this.subFolder = subFolder;

            targetFolder = parentFolder + "/" + subFolder;
        }

        public string GetDisplayText(string input)
        {
            string inputLowered = input.ToLower();
            string targetLowered = targetFolder.ToLower();

            int boldEndIndex = 0;
            for (int i = parentFolder.Length + 1; i < Mathf.Min(inputLowered.Length, targetLowered.Length); i++)
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

            string boldText = targetFolder.Substring(parentFolder.Length + 1, boldEndIndex);
            string unboldedText = targetFolder.Substring(parentFolder.Length + 1 + boldEndIndex);

            return "<color=#888>" + parentFolder + "/</color>" + boldText + "<color=#888>" + unboldedText + "</color>";
        }
        public string GetFullPath()
        {
            return parentFolder + "/" + subFolder;
        }
        public int GetMatchesCount(string input)
        {
            int matches = 0;

            input = input.ToLower();
            string targetFolderLowered = targetFolder.ToLower();

            if (input.EndsWith("/")) return 1;

            for (int i = parentFolder.Length + 1; i < Mathf.Min(input.Length, targetFolder.Length); i++)
            {
                if (input[i] == targetFolderLowered[i])
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