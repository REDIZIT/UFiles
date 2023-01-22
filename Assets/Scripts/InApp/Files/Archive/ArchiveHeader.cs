using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using TMPro;
using UnityEngine;

namespace InApp.UI
{
    public class ArchiveHeader : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI pathText;
    }
    public class ArchiveViewer
    {
        private List<ArchivePath> opennedArchives = new List<ArchivePath>();

        public static bool IsArchive(string path)
        {
            string ext = Path.GetExtension(path);
            return ext == ".rar" || ext == ".zip";
        }
        public IPath OpenArchive(string pathToArchive)
        {
            string tempFolder = Application.temporaryCachePath + "/" + Path.GetFileNameWithoutExtension(pathToArchive);
            Debug.Log("Extract archive to " + tempFolder);

            Directory.Delete(tempFolder, true);

            ZipFile.ExtractToDirectory(pathToArchive, tempFolder);
            return new ArchivePath(pathToArchive, tempFolder);
        }
    }
    public class ArchivePath : IPath
    {
        private string pathToArchive, pathToTempFolder;

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
            //throw new System.Exception("Not implemented set, path = " + path);
            pathToTempFolder = path;
        }
    }
}