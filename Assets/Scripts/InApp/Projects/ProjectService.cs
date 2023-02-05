using System.Linq;
using UnityEngine;
using Zenject;

namespace InApp
{
    public class ProjectService
    {
        [Inject] private Settings settings;

        public Project TryGetProjectAt(string currentFolder)
        {
            return settings.projects.projects.FirstOrDefault(p => p.mainFolder == currentFolder);
        }
    }
}