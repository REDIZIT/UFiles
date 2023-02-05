using System.IO;
using TMPro;
using UnityEngine;
using Zenject;

namespace InApp
{
    public class ProjectSidebarUI : MonoBehaviour
    {
        [SerializeField] private GameObject group;
        [SerializeField] private TextMeshProUGUI projectName;
        [SerializeField] private Transform urlsContent;
        [SerializeField] private UrlButton urlPrefab;
        [SerializeField] private GameObject addNewUrlButton;

        private Project currentProject;

        private ProjectService projects;
        private TabUI tabs;
        private UIHelper uiHelper;

        [Inject]
        private void Construct(ProjectService projects, TabUI tabs, UIHelper uiHelper)
        {
            this.projects = projects;
            this.tabs = tabs;
            this.uiHelper = uiHelper;

            tabs.onActiveTabPathChanged += OnPathChanged;
        }
        private void OnDestroy()
        {
            tabs.onActiveTabPathChanged -= OnPathChanged;
        }
        public void RunBuild()
        {
            projects.RunBuild(currentProject);
        }
        private void OnPathChanged()
        {
            currentProject = projects.TryGetProjectAt(tabs.ActiveTab.Folder.GetFullPath());

            group.SetActive(currentProject != null);

            if (currentProject != null)
            {
                projectName.text = Path.GetFileName(currentProject.mainFolder);
                uiHelper.Refresh(currentProject.links, urlPrefab, urlsContent);
                //uiHelper.Append(addNewUrlButton, urlsContent);
            }
        }
    }
}