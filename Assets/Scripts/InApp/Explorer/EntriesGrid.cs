using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

namespace InApp.UI
{
    public class EntriesGrid : MonoBehaviour
    {
        private int TopBorderIndex => (int)(contentRect.anchoredPosition.y / itemHeight);

        [SerializeField] private float itemHeight, spacing;

        [Inject] private EntryUIItem.Pool itemsCreationPool;

        private EntryUIItem[] pool;
        private List<EntryUIItemModel> models = new List<EntryUIItemModel>();
        private RectTransform contentRect;
        private int framesToSkip;

        private void Awake()
        {
            contentRect = GetComponent<RectTransform>();

            pool = new EntryUIItem[20];
            for (int i = 0; i < pool.Length; i++)
            {
                pool[i] = itemsCreationPool.Spawn();
                pool[i].transform.parent = contentRect;
            }
        }
        private void Update()
        {
            for (int i = 0; i < pool.Length; i++)
            {
                EntryUIItem item = pool[i];
                int modelIndex = TopBorderIndex + i;

                if (modelIndex >= models.Count)
                {
                    item.gameObject.SetActive(false);
                }
                else
                {
                    item.gameObject.SetActive(framesToSkip <= 0);

                    SetHeight(item, modelIndex * itemHeight);

                    item.Refresh(models[modelIndex]);

                    if (framesToSkip == 0)
                    {
                        item.PlayShowAnimation();
                    }
                }
            }
            if (framesToSkip >= 0)
            {
                framesToSkip--;
            }
        }
        public void Recalculate(Folder folder)
        {
            models.Clear();
            foreach (Entry entry in folder.GetEntries())
            {
                models.Add(new EntryUIItemModel(entry));
            }
            Recalculate();

            framesToSkip = 1;
        }
        public void ClearItems()
        {
            models.Clear();
        }
        private void Recalculate()
        {
            contentRect.sizeDelta = new Vector2(0, models.Count * itemHeight);
        }
        private void SetHeight(EntryUIItem item, float height)
        {
            item.Rect.anchoredPosition = new Vector2(0, -height);
            item.Rect.offsetMin = new Vector2(0, item.Rect.offsetMin.y);
            item.Rect.offsetMax = new Vector2(0, item.Rect.offsetMax.y);
        }
    }
}