using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace InApp.UI
{
    public class ContextMenuCreator : MonoBehaviour
    {
        [SerializeField] private Transform content;
        [SerializeField] private GameObject locker;

        private ContextMenuUI.Pool pool;
        private List<ContextMenuUI> menues = new List<ContextMenuUI>();
        private FilesView files;
        private DiContainer container;

        [Inject]
        private void Construct(ContextMenuUI.Pool pool, FilesView files, DiContainer container)
        {
            this.pool = pool;
            this.files = files;
            this.container = container;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                HideMenu();
            }
        }
        public ContextMenuUI ShowMenu(List<ContextItem> items, Vector2 screenPosition)
        {
            foreach (var item in items)
            {
                container.Inject(item);
            }

            var menu = pool.Spawn(items);
            menu.transform.parent = transform;
            menu.transform.position = screenPosition;
            menues.Add(menu);
            locker.SetActive(true);

            return menu;
        }
        public void HideMenu(ContextMenuUI menu)
        {
            pool.Despawn(menu);
            menues.Remove(menu);
        }
        public void OnLockerClicked()
        {
            HideMenu();
        }
        public void OnItemClicked(ContextItem item)
        {
            HideMenu();

            var env = new ContextItemEnvironment()
            {
                currentFolder = files.CurrentPath,
                selectedFiles = files.EnumerateSelectedFIles()
            };
            item.OnClick(env);
        }

        private void HideMenu()
        {
            foreach (var menu in menues)
            {
                pool.Despawn(menu);
            }

            locker.SetActive(false);
            menues.Clear();
        }
    }
}