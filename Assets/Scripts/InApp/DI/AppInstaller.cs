using InApp.Sidebar;
using InApp.UI;
using UnityEngine;
using Zenject;

namespace InApp.DI
{
    public class AppInstaller : MonoInstaller
    {
        [SerializeField] private IconsSO icons;

        [Header("Components")]
        [SerializeField] private FilesView filesView;
        [SerializeField] private ContextMenuCreator contextMenu;
        [SerializeField] private DownloadsWatcherUI downloads;
        [SerializeField] private TabUI tabs;
        [SerializeField] private AppDragDrop dragDrop;
        [SerializeField] private PicturePreview picturePreview;
        [SerializeField] private PathBar pathBar;

        [Header("Prefabs")]
        [SerializeField] private EntryUIItem entryPrefab;
        [SerializeField] private PathBarSegment pathBarSegment;
        [SerializeField] private ContextMenuUIItem contextMenuItemPrefab;
        [SerializeField] private ContextMenuUI contextMenuPrefab;
        [SerializeField] private DownloadUIItem downloadPrefab;
        [SerializeField] private SidebarFolder sidebarFolderPrefab;

        [SerializeField] private UrlButton urlButtonPrefab;

        [SerializeField] private TabUIItem tabPrefab;

        [SerializeField] private Transform entriesPool;

        [Header("Windows")]
        [SerializeField] private CreateRenameWindow createRename;
        [SerializeField] private CopyConflictWindow copyConflict;

        public override void InstallBindings()
        {
            Texture.allowThreadedTextureCreation = true;

            Container.BindInstance(icons);

            InstallSettings();

            BindInstances();
            
            Container.Bind<FileOperator>().AsSingle();
            Container.Bind<UClipboard>().AsSingle();

            Container.BindInterfacesAndSelfTo<FilePreview>().AsSingle();

            BindUIItems();
            
            Container.BindFactory<SidebarGroup, string, Transform, SidebarFolder, SidebarFolder.Factory>().FromComponentInNewPrefab(sidebarFolderPrefab).AsSingle();

            BindWindows();


            Container.Bind<Bridge>().AsSingle();

            Container.BindInterfacesAndSelfTo<ArchiveViewer>().AsSingle();
        }
        private void InstallSettings()
        {
            SettingsManager manager = new SettingsManager();
            Container.BindInstance(manager);
            manager.InstallBindings(Container);
        }
        private void BindInstances()
        {
            Container.BindInstance(filesView);
            Container.BindInstance(contextMenu);
            Container.BindInstance(downloads);
            Container.BindInstance(tabs);
            Container.BindInstance(dragDrop);
            Container.BindInstance(picturePreview);
            Container.BindInstance(pathBar);
        }
        private void BindWindows()
        {
            Container.BindInstance(createRename);
            Container.BindInstance(copyConflict);
        }

        private void BindUIItems()
        {
            BindPool<EntryUIItem, EntryUIItem.Pool>(entryPrefab);
            BindPool<PathBarSegment, PathBarSegment.Pool>(pathBarSegment);

            BindPool<ContextMenuUIItem, ContextMenuUIItem.Pool>(contextMenuItemPrefab);
            BindPool<ContextMenuUI, ContextMenuUI.Pool>(contextMenuPrefab);

            BindPool<DownloadUIItem, DownloadUIItem.Pool>(downloadPrefab);
            BindPool<UrlButton, UrlButton.Pool>(urlButtonPrefab);

            BindPool<TabUIItem, TabUIItem.Pool>(tabPrefab);
        }
        private void BindPool<Item, Pool>(Item prefab) where Item : MonoBehaviour where Pool : IMemoryPool
        {
            Container.BindMemoryPool<Item, Pool>().FromComponentInNewPrefab(prefab).UnderTransform(entriesPool);
        }
    }
}