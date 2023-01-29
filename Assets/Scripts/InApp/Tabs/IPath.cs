using UnityEngine;

namespace InApp
{
    public interface IPath
    {
        void Set(string path);
        string GetDisplayName();
        string GetFullPath();
    }
}