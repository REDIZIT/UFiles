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

        [SerializeField] private Transform entriesPool;

        [Header("Windows")]
        [SerializeField] private CreateRenameWindow createRename;

        public override void InstallBindings()
        {
            Texture.allowThreadedTextureCreation = true;

            Container.BindInstances(icons);

            Container.BindInstance(filesView);
            Container.BindInstance(contextMenu);
            Container.BindInstance(downloads);

            Container.BindMemoryPool<EntryUIItem, EntryUIItem.Pool>().FromComponentInNewPrefab(entryPrefab).UnderTransform(entriesPool);
            Container.BindMemoryPool<PathBarSegment, PathBarSegment.Pool>().FromComponentInNewPrefab(pathBarSegment);

            Container.BindMemoryPool<ContextMenuUIItem, ContextMenuUIItem.Pool>().FromComponentInNewPrefab(contextMenuItemPrefab).UnderTransform(entriesPool);
            Container.BindMemoryPool<ContextMenuUI, ContextMenuUI.Pool>().FromComponentInNewPrefab(contextMenuPrefab).UnderTransform(entriesPool);

            BindPool<DownloadUIItem, DownloadUIItem.Pool>(downloadPrefab);

            BindWindows();
        }
        private void BindWindows()
        {
            Container.BindInstances(createRename);
        }

        private void BindPool<Item, Pool>(Item prefab) where Item : MonoBehaviour where Pool : IMemoryPool
        {
            Container.BindMemoryPool<Item, Pool>().FromComponentInNewPrefab(prefab).UnderTransform(entriesPool);
        }
    }
}