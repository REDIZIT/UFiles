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
        private ContextMenuUI menu;
        private FilesView files;

        [Inject]
        private void Construct(ContextMenuUI.Pool pool, FilesView files)
        {
            this.pool = pool;
            this.files = files;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                HideMenu();
            }
        }
        public void ShowMenu(List<ContextItem> items)
        {
            if (menu != null)
            {
                HideMenu();
            }
            else
            {
                menu = pool.Spawn(items);
                menu.transform.parent = transform;
                menu.transform.position = Input.mousePosition;
                locker.SetActive(true);
            }
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
            if (menu == null) return;

            pool.Despawn(menu);
            locker.SetActive(false);
            menu = null;
        }
    }
}