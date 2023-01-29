using InApp.UI;
using System;
using Zenject;

namespace InApp
{
    public class Tab
    {
        public Folder Folder { get; private set; }

        public Action onPathChanged;

        private History<Folder> history = new History<Folder>(HistoryPointerType.TargetFrame);

        [Inject] private ArchiveViewer archiveViewer;

        public Tab(Folder startFolder)
        {
            Folder = startFolder;
            history.Add(Folder);
        }

        public void Open(Entry entry)
        {
            Folder = Folder.Open(entry);

            history.Add(Folder);

            onPathChanged?.Invoke();
        }
        public void TryUndo()
        {
            if (history.TryUndo(out Folder folder))
            {
                Folder = folder;
                onPathChanged?.Invoke();
            }
        }
        public void TryRedo()
        {
            if (history.TryRedo(out Folder folder))
            {
                Folder = folder;
                onPathChanged?.Invoke();
            }
        }
    }
}