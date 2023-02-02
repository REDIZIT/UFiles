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
                pool[i].transform.SetParent(contentRect);
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

            ClearItems();

            thread = new Thread(LoadModels);
            thread.Start();

            framesToSkip = 1;
        }
        public void ClearItems()
        {
            models.Clear();
            Update();
        }

        private void LoadModels()
        {
            models.Clear();
            foreach (Entry entry in folder.GetEntries())
            {
                models.Add(new EntryUIItemModel(entry, folder));
            }
        }
        private void UpdateUIItems()
        {
            float currentHeight = 0;
            int poolIndex = 0;
            for (int i = 0; i < models.Count; i++)
            {
                EntryUIItemModel model = models[i];

                float nextHeight = currentHeight + model.Height + 2;

                bool isVisible = nextHeight >= contentRect.anchoredPosition.y;

                if (isVisible && poolIndex < pool.Length)
                {
                    EntryUIItem item = pool[poolIndex];
                    poolIndex++;

                    item.gameObject.SetActive(framesToSkip <= 0);

                    SetHeight(item, currentHeight);

                    item.Refresh(model);

                    if (framesToSkip == 0)
                    {
                        item.PlayShowAnimation();
                    }
                }

                currentHeight = nextHeight;
            }

            // Hide unused items
            while (poolIndex < pool.Length - 1)
            {
                poolIndex++;
                pool[poolIndex].gameObject.SetActive(false);
            }
        }
        private void FitContentSize()
        {
            float height = 0;
            for (int i = 0; i < models.Count; i++)
            {
                if (models[i] == null) continue;
                height += models[i].Height + 2;
            }
            contentRect.sizeDelta = new Vector2(0, height);
        }
        private void SetHeight(EntryUIItem item, float height)
        {
            item.Rect.anchoredPosition = new Vector2(0, -height);
            item.Rect.offsetMin = new Vector2(0, item.Rect.offsetMin.y);
            item.Rect.offsetMax = new Vector2(0, item.Rect.offsetMax.y);
        }
    }
}