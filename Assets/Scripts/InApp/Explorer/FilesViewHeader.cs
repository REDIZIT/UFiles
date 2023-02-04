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

        public void OnHeaderClicked(int typeNumber)
        {
            FolderSortingData.Type type = (FolderSortingData.Type)typeNumber;
            var data = settings.folderSortingData.FirstOrDefault(d => d.path == files.CurrentPath);
            if (data != null)
            {
                if (data.type == type)
                {
                    if (data.isReversed == false)
                    {
                        data.isReversed = true;
                    }
                    else
                    {
                        data.type = FolderSortingData.Type.None;
                        data.isReversed = false;
                    }
                }
                else
                {
                    data.type = type;
                    data.isReversed = false;
                }
            }
            else
            {
                data = new FolderSortingData()
                {
                    path = files.CurrentPath,
                    type = type,
                    isReversed = true
                };
                settings.folderSortingData.Add(data);
            }

            files.RefreshWithoutAnimations();
            settings.Save();
        }
    }

}