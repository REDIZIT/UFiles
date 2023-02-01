using InApp.UI;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Profiling;
using Zenject;

namespace InApp
{
    public class TabUI : MonoBehaviour, IPointerExitHandler
    {
        public Tab ActiveTab { get; private set; }
        public float TabWidth { get; private set; }

        public Action onActiveTabChanged;
        public Action onActiveTabPathChanged;

        [SerializeField] private RectTransform tabsContent;
        [SerializeField] private RectTransform addButton;
        [SerializeField] private WindowScript window;

        private List<Tab> tabs = new List<Tab>();
        private DateTime d1;

        private TabUIItem.Pool pool;
        private FilesView files;
        private DiContainer container;

        [Inject]
        private void Construct(DiContainer container, TabUIItem.Pool pool, FilesView files)
        {
            this.container = container;
            this.pool = pool;
            this.files = files;
        }

        private void Start()
        {
            OnAddTabClicked();
        }
        private void Update()
        {
            if (d1 != default)
            {
                Debug.Log("Openned in " + (DateTime.Now - d1).TotalMilliseconds + "ms");
                d1 = default;
            }

            // History
            if (Input.GetMouseButtonDown(3))
            {
                ActiveTab.TryUndo();
            }
            if (Input.GetMouseButtonDown(4))
            {
                ActiveTab.TryRedo();
            }
        }

        public void OpenNew(Folder folder, bool switchToNew)
        {
            container.Inject(folder);
            Tab model = new Tab(folder, container);
            container.Inject(model);

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
            //OpenNew(new LocalFolder("C:/Users/redizit/Downloads"), true);
            d1 = DateTime.Now;
            Profiler.BeginSample("Open tab");
            OpenNew(new LocalFolder("C:/Windows/WinSxS"), true);
            Profiler.EndSample();
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
            if (ActiveTab != null)
            {
                ActiveTab.onPathChanged -= OnActiveTabPathChanged;
            }

            ActiveTab = tab;

            ActiveTab.onPathChanged += OnActiveTabPathChanged;

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
        private void OnActiveTabPathChanged()
        {
            onActiveTabPathChanged?.Invoke();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            RecalculateTabWidth();
        }
    }
}