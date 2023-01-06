using System.Linq;
using UnityEngine;
using Zenject;

namespace InApp.UI
{
    public class FilesViewHeader : MonoBehaviour
    {
        private FilesView files;
        private Settings settings;

        [Inject]
        private void Construct(FilesView files, Settings settings)
        {
            this.files = files;
            this.settings = settings;
        }

        public void OnHeaderClicked()
        {
            var data = settings.folderSortingData.FirstOrDefault(d => d.path == files.CurrentPath);
            if (data != null)
            {
                data.isSortingByDate = !data.isSortingByDate;
            }
            else
            {
                data = new FolderSortingData()
                {
                    path = files.CurrentPath,
                    isSortingByDate = true
                };
                settings.folderSortingData.Add(data);
            }

            files.Refresh();
            settings.Save();
        }
    }

}