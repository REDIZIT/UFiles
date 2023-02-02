namespace InApp.UI
{
    public interface IPathBarHint
    {
        string GetDisplayText(string input);
        string GetFullPath();
        int GetMatchesCount(string input);
    }
}