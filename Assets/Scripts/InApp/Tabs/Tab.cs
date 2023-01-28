using InApp.UI;
using System;
using Zenject;

namespace InApp
{
    public class Tab
    {
        public IPath path;

        public Action onPathChanged;

        private History<string> history = new History<string>(HistoryPointerType.TargetFrame);

        [Inject] private ArchiveViewer archiveViewer;

        public Tab(IPath path)
        {
            this.path = path;
            history.Add(path.GetFullPath());
        }

        public void Open(string entryFullPath, bool saveToHistory = true)
        {
            if (ArchiveViewer.IsArchive(entryFullPath))
            {
                path = archiveViewer.OpenArchive(entryFullPath);
            }
            else
            {
                path = new EntryPath(entryFullPath);

                if (saveToHistory)
                {
                    history.Add(entryFullPath);
                }
            }

            onPathChanged?.Invoke();
        }
        public void TryUndo()
        {
            if (history.TryUndo(out string path))
            {
                Open(path, false);
            }
        }
        public void TryRedo()
        {
            if (history.TryRedo(out string path))
            {
                Open(path, false);
            }
        }
    }
}