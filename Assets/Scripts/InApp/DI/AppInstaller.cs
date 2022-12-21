using InApp.UI;
using UnityEngine;
using Zenject;

namespace InApp.DI
{
    public class AppInstaller : MonoInstaller
    {
        [SerializeField] private IconsSO icons;
        [SerializeField] private FilesView filesView;
        [SerializeField] private EntryUIItem entryPrefab;
        [SerializeField] private PathBarSegment pathBarSegment;

        [SerializeField] private Transform entriesPool;

        public override void InstallBindings()
        {
            Container.BindInstances(icons);
            Container.BindInstance(filesView);

            Container.BindMemoryPool<EntryUIItem, EntryUIItem.Pool>().FromComponentInNewPrefab(entryPrefab).UnderTransform(entriesPool);
            Container.BindMemoryPool<PathBarSegment, PathBarSegment.Pool>().FromComponentInNewPrefab(pathBarSegment);
        }
    }
}