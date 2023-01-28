using System.IO;

namespace InApp.UI
{
    public class ArchivePath : IPath
    {
        public string pathToArchive, pathToTempFolder, localPath;

        public ArchivePath(string pathToArchive, string pathToTempFolder)
        {
            this.pathToArchive = pathToArchive;
            this.pathToTempFolder = pathToTempFolder;
        }

        public string GetDisplayName()
        {
            return Path.GetFileNameWithoutExtension(pathToArchive);
        }

        public string GetFullPath()
        {
            return pathToTempFolder;
        }

        public void Set(string path)
        {
            pathToTempFolder = path;
        }
    }
}