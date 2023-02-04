using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace InApp.UI
{
    public class ContextMenuUI : MonoBehaviour
    {
        public float Height => layoutGroup.padding.top + layoutGroup.padding.bottom + spawnedItems.Count * 22;

        public RectTransform rect;

        [SerializeField] private RectTransform content;
        [SerializeField] private VerticalLayoutGroup layoutGroup;

        private ContextMenuUIItem.Pool pool;
        private List<ContextMenuUIItem> spawnedItems = new List<ContextMenuUIItem>();

        [Inject]
        private void Construct(ContextMenuUIItem.Pool pool)
        {
            this.pool = pool;
        }

        private void Refresh(List<ContextItem> items, Vector2 pos)
        {
            foreach (var item in spawnedItems)
            {
                pool.Despawn(item);
            }
            spawnedItems.Clear();

            foreach (var item in items)
            {
                var inst = pool.Spawn(item);
                inst.transform.SetParent(content);
                spawnedItems.Add(inst);
            }

            
            if (pos.y - Height < 0)
            {
                content.pivot = new Vector2(0.5f, 0);
            }
            else
            {
                content.pivot = new Vector2(0.5f, 1);
            }

            content.anchoredPosition = Vector2.zero;
            transform.position = pos;
        }

        public class Pool : MonoMemoryPool<List<ContextItem>, Vector2, ContextMenuUI>
        {
            protected override void Reinitialize(List<ContextItem> p1, Vector2 pos, ContextMenuUI item)
            {
                base.Reinitialize(p1, pos, item);
                item.Refresh(p1, pos);
            }
        }
    }
}