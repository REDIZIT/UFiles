using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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
        private IEnumerator<Entry> enumerator;
        private List<EntryUIItemModel> models = new List<EntryUIItemModel>();

        private Thread thread;
        private Folder folder;
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
            UpdateUIItems();
            FitContentSize();

            if (framesToSkip >= 0)
            {
                framesToSkip--;
            }
        }
        public void Recalculate(Folder folder)
        {
            this.folder = folder;

            thread = new Thread(LoadModels);
            thread.Start();

            framesToSkip = 1;
        }
        public void ClearItems()
        {
            models.Clear();
        }

        private void LoadModels()
        {
            models.Clear();
            foreach (Entry entry in folder.GetEntries())
            {
                models.Add(new EntryUIItemModel(entry));
            }
        }
        private void UpdateUIItems()
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
        }
        private void FitContentSize()
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