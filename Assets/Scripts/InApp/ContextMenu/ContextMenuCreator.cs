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
        private CreateRenameWindow createRenameWindow;

        [Inject]
        private void Construct(ContextMenuUI.Pool pool, FilesView files, CreateRenameWindow createRenameWindow)
        {
            this.pool = pool;
            this.files = files;
            this.createRenameWindow = createRenameWindow;
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(1))
            {
                if (menu != null)
                {
                    HideMenu();
                }
                else
                {
                    menu = pool.Spawn(new List<ContextItem>()
                    { 
                        new CreateFileItem(createRenameWindow, files) { text = "Create file" } 
                    });
                    menu.transform.parent = transform;
                    menu.transform.position = Input.mousePosition;

                    locker.SetActive(true);
                }
            }
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                HideMenu();
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
                currentFolder = files.CurrentPath
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