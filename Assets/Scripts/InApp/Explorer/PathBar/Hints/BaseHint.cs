using UnityEngine;

namespace InApp.UI
{
    public abstract class BaseHint : IPathBarHint
    {
        protected IconsSO icons;

        public BaseHint(IconsSO icons)
        {
            this.icons = icons;
        }

        public abstract string GetDisplayText(string input);
        public abstract string GetFullPath();
        public abstract Sprite GetIcon();

        public float GetMatchesCount(string input)
        {
            return GetTextMatches(GetDisplayText(input), input);
        }

        public abstract string GetTypeText();

        protected float GetTextMatches(string text, string input)
        {
            int matches = 0;

            input = input.ToLower();
            string displayTextLowered = text.ToLower();

            for (int i = 0; i < Mathf.Min(input.Length, text.Length); i++)
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