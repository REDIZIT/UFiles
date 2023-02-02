using UnityEngine;

namespace InApp.UI
{
    public interface IPathBarHint
    {
        string GetDisplayText(string input);
        string GetFullPath();
        string GetTypeText();
        int GetMatchesCount(string input);
        Sprite GetIcon();
    }
}