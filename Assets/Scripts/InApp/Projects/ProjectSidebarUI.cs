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
        private void OnPathChanged()
        {
            Project project = projects.TryGetProjectAt(tabs.ActiveTab.Folder.GetFullPath());

            group.SetActive(project != null);

            if (project != null)
            {
                projectName.text = Path.GetFileName(project.mainFolder);
                uiHelper.Refresh(project.links, urlPrefab, urlsContent);
                uiHelper.Append(addNewUrlButton, urlsContent);
            }
        }
    }
}