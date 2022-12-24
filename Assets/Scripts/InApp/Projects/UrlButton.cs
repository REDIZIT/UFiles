using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace InApp
{
    [System.Serializable]
    public class Project
    {
        public string mainFolder;
        public string buildPath;
        public List<ProjectLink> links = new List<ProjectLink>();
    }
    public class UrlButton : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI text;
        [SerializeField] private Image image;

        private ProjectLink link;

        private void Start()
        {
            text.text = link.displayText;
        }

        public void OnClick()
        {
            Application.OpenURL(link.url);
        }

        public class Pool : MonoMemoryPool<ProjectLink, UrlButton>
        {
            protected override void Reinitialize(ProjectLink p1, UrlButton item)
            {
                item.link = p1;
                base.Reinitialize(p1, item);
            }
        }
    }
    [System.Serializable]
    public class ProjectLink
    {
        public string displayText, url;
        public Sprite icon;
    }
}