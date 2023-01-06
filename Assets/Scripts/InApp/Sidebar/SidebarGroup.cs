using InApp.UI;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Zenject;

namespace InApp.Sidebar
{
    public class SidebarGroup : MonoBehaviour, AppDragDropTarget
    {
        [SerializeField] private RectTransform content;
        [SerializeField] private GameObject insertItem;
        [SerializeField] private Sprite starIcon;

        private List<SidebarFolder> items = new List<SidebarFolder>();

        [Inject] SidebarFolder.Factory factory;
        [Inject] Settings settings;

        private void Start()
        {
            Refresh();
        }
        public void Refresh()
        {
            foreach (var item in items)
            {
                Destroy(item.gameObject);
            }
            items.Clear();

            foreach (string entry in settings.sidebar.favourite)
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

        private int GetInsertIndex()
        {
            float itemHeight = 26;
            float spacing = 2;

            float localY = content.position.y + content.rect.height / 2f - Input.mousePosition.y;
            return (int)((localY + itemHeight / 2f) / (itemHeight + spacing));
        }
    }
}