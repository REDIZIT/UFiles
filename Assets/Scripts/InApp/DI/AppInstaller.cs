using InApp.UI;
using UnityEngine;
using Zenject;

namespace InApp.DI
{
    public class AppInstaller : MonoInstaller
    {
        [SerializeField] private IconsSO icons;

        [SerializeField] private FilesView filesView;
        [SerializeField] private ContextMenuCreator contextMenu;
        [SerializeField] private DownloadsWatcherUI downloads;

        [SerializeField] private EntryUIItem entryPrefab;
        [SerializeField] private PathBarSegment pathBarSegment;
        [SerializeField] private ContextMenuUIItem contextMenuItemPrefab;
        [SerializeField] private ContextMenuUI contextMenuPrefab;
        [SerializeField] private DownloadUIItem downloadPrefab;

        [SerializeField] private UrlButton urlButtonPrefab;

        [SerializeField] private Transform entriesPool;

        [Header("Windows")]
        [SerializeField] private CreateRenameWindow createRename;
        [SerializeField] private CopyConflictWindow copyConflict;

        public override void InstallBindings()
        {
            Texture.allowThreadedTextureCreation = true;

            Container.BindInstances(icons);

            Container.BindInstance(filesView);
            Container.BindInstance(contextMenu);
            Container.BindInstance(downloads);

            Container.Bind<FileOperator>().AsSingle();
            Container.Bind<UClipboard>().AsSingle();

            BindPool<EntryUIItem, EntryUIItem.Pool>(entryPrefab);
            BindPool<PathBarSegment, PathBarSegment.Pool>(pathBarSegment);

            BindPool<ContextMenuUIItem, ContextMenuUIItem.Pool>(contextMenuItemPrefab);
            BindPool<ContextMenuUI, ContextMenuUI.Pool>(contextMenuPrefab);

            BindPool<DownloadUIItem, DownloadUIItem.Pool>(downloadPrefab);
            BindPool<UrlButton, UrlButton.Pool>(urlButtonPrefab);

            BindWindows();
        }
        private void BindWindows()
        {
            Container.BindInstances(createRename);
            Container.BindInstances(copyConflict);
        }

        private void BindPool<Item, Pool>(Item prefab) where Item : MonoBehaviour where Pool : IMemoryPool
        {
            Container.BindMemoryPool<Item, Pool>().FromComponentInNewPrefab(prefab).UnderTransform(entriesPool);
        }
    }
}