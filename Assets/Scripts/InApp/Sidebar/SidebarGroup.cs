using InApp.UI;
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

        [Inject] SidebarFolder.Factory factory;
        [Inject] Settings settings;

        private void Start()
        {
            foreach (string entry in settings.sidebar.favourite)
            {
                factory.Create(entry, content);
            }

            insertItem.gameObject.SetActive(false);
        }

        public bool WillTake(DragDropData data)
        {
            return data is EntryUIItem.EntryData entryData && Directory.Exists(entryData.path);
        }
        public void OnMouseOver(DragDropData data)
        {
            insertItem.gameObject.SetActive(true);

            float itemHeight = 26;
            float spacing = 2;

            float localY = content.position.y + content.rect.height / 2f - Input.mousePosition.y;
            int index = (int)((localY + itemHeight / 2f) / (itemHeight + spacing));

            insertItem.transform.SetSiblingIndex(index);
        }

        public void OnDrop(DragDropData item)
        {
            var data = (EntryUIItem.EntryData)item;
            factory.Create(data.path, content);
            settings.sidebar.favourite.Add(data.path);
            settings.Save();
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
    }
}