using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

namespace InApp
{
    public class DownloadsWatcher
    {
        public static class KnownFolder
        {
            public static readonly Guid Downloads = new Guid("374DE290-123F-4565-9164-39C4925E467B");
        }

        [DllImport("shell32.dll", CharSet = CharSet.Unicode)]
        static extern int SHGetKnownFolderPath([MarshalAs(UnmanagedType.LPStruct)] Guid rfid, uint dwFlags, IntPtr hToken, out string pszPath);

        public List<string> changedFiles = new();
        
        private FileSystemWatcher watcher;

        public DownloadsWatcher()
        {
            watcher = new();
            watcher.Path = GetDownloadsPath();
            watcher.NotifyFilter = NotifyFilters.CreationTime;
            watcher.Filter = "*.*";
            watcher.Created += new FileSystemEventHandler(OnDownloadsChange);
            watcher.EnableRaisingEvents = true;
        }


        private string GetDownloadsPath()
        {
            SHGetKnownFolderPath(KnownFolder.Downloads, 0, IntPtr.Zero, out string path);
            return path;
        }
        private void OnDownloadsChange(object sender, FileSystemEventArgs e)
        {
            if (Path.GetExtension(e.FullPath) == "~") return;

            changedFiles.Add(e.FullPath);
        }
    }
}