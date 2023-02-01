using InApp.UI;
using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace InApp
{
    public class Tab
    {
        public Folder Folder { get; private set; }
        public bool IsLoading { get; private set; }

        public Action onPathChanged;

        private History<Folder> history = new History<Folder>(HistoryPointerType.TargetFrame);
        private DiContainer container;
        private Thread folderOpenThread;
        private FilesView files;

        public Tab(Folder startFolder, DiContainer container)
        {
            this.container = container;
            Folder = startFolder;
            history.Add(Folder);

            files = container.Resolve<FilesView>();
        }

        public void Open(Entry entry)
        {
            if (IsLoading) return;

            IsLoading = true;
            files.ShowLoading("Папка открывается");

            folderOpenThread = new Thread(() =>
            {
                Folder = Folder.Open(entry);

                UnityMainThreadDispatcher.Instance().Enqueue(() =>
                {
                    history.Add(Folder);

                    onPathChanged?.Invoke();
                    IsLoading = false;
                });
            });

            

            folderOpenThread.Start();
        }
        public void Open(string localFolderPath)
        {
            Folder = new LocalFolder(localFolderPath);
            container.Inject(Folder);

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
        public void TryUndoUntil(Func<Folder, bool> func)
        {
            while (history.TryUndo(out Folder folder))
            {
                if (func(folder))
                {
                    Folder = folder;
                    onPathChanged?.Invoke();
                    break;
                }
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