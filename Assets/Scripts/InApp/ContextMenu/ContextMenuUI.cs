using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace InApp.UI
{
    public class ContextMenuUI : MonoBehaviour
    {
        public RectTransform rect;

        [SerializeField] private Transform content;

        private ContextMenuUIItem.Pool pool;
        private List<ContextMenuUIItem> spawnedItems = new List<ContextMenuUIItem>();

        [Inject]
        private void Construct(ContextMenuUIItem.Pool pool)
        {
            this.pool = pool;
        }

        private void Refresh(List<ContextItem> items)
        {
            foreach (var item in spawnedItems)
            {
                pool.Despawn(item);
            }
            spawnedItems.Clear();

            foreach (var item in items)
            {
                var inst = pool.Spawn(item);
                inst.transform.parent = content;
                spawnedItems.Add(inst);
            }
        }

        public class Pool : MonoMemoryPool<List<ContextItem> , ContextMenuUI>
        {
            protected override void Reinitialize(List<ContextItem> p1, ContextMenuUI item)
            {
                base.Reinitialize(p1, item);
                item.Refresh(p1);
            }
        }
    }
}