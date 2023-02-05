using System.IO;
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
            return settings.projects.projects.FirstOrDefault(p => currentFolder.StartsWith(p.mainFolder));
        }
        public void RunBuild(Project project)
        {
            string fullPath = project.mainFolder + "/" + project.buildExecutable;
            Debug.Log(fullPath);
            if (File.Exists(fullPath))
            {
                System.Diagnostics.Process.Start(fullPath);
            }
        }
    }
}