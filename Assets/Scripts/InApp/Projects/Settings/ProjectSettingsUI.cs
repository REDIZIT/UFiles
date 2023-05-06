using UnityEngine;
using Zenject;

namespace InApp
{
    public class ProjectSettingsUI : MonoBehaviour
    {
        [SerializeField] private ProjectSettingsUILot prefab;
        [SerializeField] private Transform projectsContent;

        [SerializeField] private SettingLot settingPrefab;
        [SerializeField] private Transform settingsContent;

        [Inject] private UIHelper helper;
        [Inject] private SettingsManager settingsManager;
        [Inject] private Settings settings;

        private Project selectedProject;

        void Start ()
        {
            helper.Refresh(settings.projects.projects, prefab, projectsContent);

            if (selectedProject != null)
            {
                //helper.Refresh(settings.projects.projects.)
            }
        }
    }
    public class ProjectSettingsUILot : UILot<Project>
    {

    }
    public class SettingLot : UILot<Option>
    {

    }
    public abstract class Option
    {
        public string name;

        public Option(string name)
        {
            this.name = name;
        }
    }
    public class InputFieldOption : Option
    {
        public string value;

        public InputFieldOption(string name, string value) : base(name)
        {
            this.value = value;
        }
    }
}