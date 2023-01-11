using InApp.UI;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using Zenject;

namespace InApp.Sidebar
{
    public class SidebarGroup : MonoBehaviour, AppDragDropTarget
    {
        [SerializeField] private RectTransform content;
        [SerializeField] private GameObject insertItem;
        [SerializeField] private Sprite starIcon;
        [SerializeField] private Transform arrow;

        [SerializeField] private float itemHeight = 26;
        [SerializeField] private float spacing = 2;

        [SerializeField] private EntriesList list;

        private List<SidebarFolder> items = new List<SidebarFolder>();
        private RectTransform rect;
        private bool isExpanded = true;

        [Inject] SidebarFolder.Factory factory;
        [Inject] Settings settings;

        [System.Serializable]
        public enum EntriesList
        {
            Favourite,
            Drives
        }

        private void Start()
        {
            rect = GetComponent<RectTransform>();
            Refresh();
        }
        private void Update()
        {
            FitSize();
            content.gameObject.SetActive(isExpanded);
            arrow.eulerAngles = new Vector3(0, 0, isExpanded ? 90 : -90);
        }
        public void Refresh()
        {
            foreach (var item in items)
            {
                Destroy(item.gameObject);
            }
            items.Clear();

            foreach (string entry in GetEntries())
            {
                items.Add(factory.Create(this, entry, content));
            }

            insertItem.gameObject.SetActive(false);
        }

        public bool WillHandleDrop(DragDropData data)
        {
            return data is EntryUIItem.EntryData entryData
                && Directory.Exists(entryData.path)
                && settings.sidebar.favourite.Contains(entryData.path) == false;
        }
        public void OnMouseOver(DragDropData data)
        {
            insertItem.gameObject.SetActive(true);

            insertItem.transform.SetSiblingIndex(GetInsertIndex());
        }

        public void OnDrop(DragDropData item)
        {
            var data = (EntryUIItem.EntryData)item;

            if (settings.sidebar.favourite.Contains(data.path)) return;

            int index = Mathf.Min(GetInsertIndex(), settings.sidebar.favourite.Count);
            settings.sidebar.favourite.Insert(index, data.path);
            settings.Save();

            Refresh();
        }

        public void OnMouseExit()
        {
            insertItem.gameObject.SetActive(false);
        }

        public Sprite GetIcon()
        {
            return starIcon;
        }

        public string GetDisplayText()
        {
            return "Добавить в избранное";
        }
        public void SwitchExpanded()
        {
            isExpanded = !isExpanded;
        }
        private void FitSize()
        {
            float size = itemHeight;

            if (isExpanded)
            {
                size += itemHeight + itemHeight * items.Count + Mathf.Max(0, spacing * (items.Count - 1));
            }

            rect.sizeDelta = new Vector2(rect.sizeDelta.x, size);
        }

        private int GetInsertIndex()
        {
            float localY = content.position.y + content.rect.height / 2f - Input.mousePosition.y;
            return (int)((localY - itemHeight / 2f - itemHeight) / (itemHeight + spacing));
        }

        private IEnumerable<string> GetEntries()
        {
            if (list == EntriesList.Favourite) return settings.sidebar.favourite;
            else if (list == EntriesList.Drives) return EntryUtils.GetDrives();
            else throw new System.Exception("Not matched list with " + list);
        }
    }
}