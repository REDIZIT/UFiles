using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

namespace InApp.UI
{
    public class EntriesGrid : MonoBehaviour
    {
        [SerializeField] private float itemHeight, spacing;

        [Inject] private EntryUIItem.Pool itemsCreationPool;

        private EntryUIItem[] pool;
        private List<EntryUIItemModel> models = new List<EntryUIItemModel>();
        private RectTransform contentRect;

        private void Awake()
        {
            contentRect = GetComponent<RectTransform>();

            pool = new EntryUIItem[1];
            for (int i = 0; i < pool.Length; i++)
            {
                pool[i] = itemsCreationPool.Spawn();
            }
        }
        public void Recalculate(Folder folder)
        {
            models.Clear();
            foreach (Entry entry in folder.GetEntries())
            {
                //var model = new EntryUIItemModel(entry);
                //models.Add(model);
            }

            Recalculate();
        }
        public void ClearItems()
        {

        }
        private void Recalculate()
        {
            float currentHeight = 0;
            for (int i = 0; i < models.Count; i++)
            {
                EntryUIItemModel model = models[i];

                //if (model.isShowed)
                //{
                //    SetHeight(model.item, currentHeight);
                //}

                currentHeight += itemHeight + spacing;
            }

            contentRect.offsetMin = new Vector2(0, -currentHeight);
        }
        private void SetHeight(EntryUIItem item, float height)
        {
            item.Rect.anchoredPosition = new Vector2(0, -height);
            item.Rect.offsetMin = new Vector2(0, item.Rect.offsetMin.y);
            item.Rect.offsetMax = new Vector2(0, item.Rect.offsetMax.y);
        }
        //private float GetHeight(EntryUIItem item)
        //{
        //    return item.Rect.sizeDelta.y;
        //}
    }
}