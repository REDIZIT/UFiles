using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
                try
                {
                    Directory.Delete(dir, true);
                }
                catch
                {

                }
            }
        }
        public bool IsInArchive(string entryPath)
        {
            return entryPath.StartsWith(archivesTempFolder);
        }

        public ArchivePath OpenArchive(string pathToArchive)
        {
            ArchivePath opennedArchive = opennedArchives.FirstOrDefault(p => p.pathToArchive == pathToArchive);
            if (opennedArchive != null) return opennedArchive;

            return ExtractArchive(pathToArchive);
        }
        public void CloseArchive(ArchivePath archive)
        {
            opennedArchives.Remove(archive);
        }

        private ArchivePath ExtractArchive(string pathToArchive)
        {
            string tempFolder = archivesTempFolder + "/" + Guid.NewGuid();
            Debug.Log("Extract archive to " + tempFolder);

            bridgeResponse = string.Empty;
            waitCount = 0;

            bridge.Enqueue(new ExtractArchiveCommand(pathToArchive, tempFolder, OnExtractCompleted));

            while (string.IsNullOrEmpty(bridgeResponse) && waitCount < 10000)
            {
                Thread.Sleep(2);
                waitCount++;
            }

            Debug.Log("Response: " + bridgeResponse + ", waited: " + (waitCount * 2) + "ms");

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