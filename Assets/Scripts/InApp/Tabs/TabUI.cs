using InApp.UI;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace InApp
{
    public class TabUI : MonoBehaviour, IPointerExitHandler
    {
        public Tab ActiveTab { get; private set; }
        public float TabWidth { get; private set; }

        public Action onActiveTabChanged;

        [SerializeField] private RectTransform tabsContent;
        [SerializeField] private RectTransform addButton;
        [SerializeField] private WindowScript window;

        private List<Tab> tabs = new List<Tab>();

        private TabUIItem.Pool pool;
        private FilesView files;

        [Inject]
        private void Construct(TabUIItem.Pool pool, FilesView files)
        {
            this.pool = pool;
            this.files = files;

            OnAddTabClicked();
        }

        public void OpenNew(IPath path, bool switchToNew)
        {
            Tab model = new Tab()
            {
                path = path
            };

            var inst = pool.Spawn(model);
            inst.transform.parent = tabsContent;

            tabs.Add(model);

            if (switchToNew)
            {
                OnTabClicked(model);
            }

            RecalculateTabWidth();
            OnTabsCountChanged();
        }

        public void OnAddTabClicked()
        {
            OpenNew(new EntryPath("C:/Users/redizit/Downloads"), true);
        }
        public void OnTabClicked(Tab model)
        {
            SwitchTab(model);
        }
        public void OnCloseTabClicked(Tab model)
        {
            int index = tabs.IndexOf(ActiveTab);

            tabs.Remove(model);

            if (index > tabs.Count - 1) index = tabs.Count - 1;

            SwitchTab(tabs[index]);

            OnTabsCountChanged();
        }
        private void SwitchTab(Tab tab)
        {
            ActiveTab = tab;
            onActiveTabChanged?.Invoke();

            files.OpenTab(tab);
        }
        private void OnTabsCountChanged()
        {
            addButton.transform.SetAsLastSibling();
        }
        private void RecalculateTabWidth()
        {
            float totalWidth = tabsContent.rect.width - addButton.rect.width;
            TabWidth = Mathf.Min(totalWidth / tabs.Count, 240);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            RecalculateTabWidth();
        }
    }
}