using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using UnityEngine;
using Zenject;

namespace InApp.UI
{
    public class ArchiveViewer : IDisposable
    {
        [Inject] private Bridge bridge;

        private string archivesTempFolder;
        private List<ArchivePath> opennedArchives = new List<ArchivePath>();
        private string bridgeResponse;
        private int waitCount;

        public static bool IsArchive(string path)
        {
            string ext = Path.GetExtension(path);
            return ext == ".rar" || ext == ".zip";
        }

        public ArchiveViewer()
        {
            archivesTempFolder = Application.temporaryCachePath + "/Archives";
        }
        public void Dispose()
        {
            foreach (string dir in Directory.EnumerateDirectories(archivesTempFolder))
            {
                Directory.Delete(dir, true);
            }
        }

        public ArchivePath OpenArchive(string pathToArchive)
        {
            string tempFolder = archivesTempFolder + "/" + Path.GetFileNameWithoutExtension(pathToArchive);
            Debug.Log("Extract archive to " + tempFolder);

            bridgeResponse = string.Empty;
            waitCount = 0;

            bridge.Enqueue(new ExtractArchiveCommand(pathToArchive, tempFolder, OnExtractCompleted));

            while(string.IsNullOrEmpty(bridgeResponse) && waitCount < 60 * 3)
            {
                Thread.Sleep(16);
                waitCount++;
            }

            Debug.Log("Response: " + bridgeResponse + ", waited: " + (waitCount * 16) + "ms");

            ArchivePath path = new ArchivePath(pathToArchive, tempFolder);
            opennedArchives.Add(path);

            return path;
        }

        private void OnExtractCompleted(string message)
        {
            bridgeResponse = message;
        }
    }
}