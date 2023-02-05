using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Zenject;
using Debug = UnityEngine.Debug;

namespace InApp
{
    public class ProjectService
    {
        private Settings settings;
        private TabUI tabs;

        [Inject]
        public void Construct(Settings settings, TabUI tabs)
        {
            this.settings = settings;
            this.tabs = tabs;
        }
        public Project TryGetActiveProject()
        {
            return TryGetProjectAt(tabs.ActiveTab.Folder.GetFullPath());
        }
        public void RunBuild(Project project)
        {
            string fullPath = project.mainFolder + "/" + project.buildExecutable;
            if (File.Exists(fullPath))
            {
                System.Diagnostics.Process.Start(fullPath);
            }
        }
        public void IndexProject(Project project)
        {
            Stopwatch w = Stopwatch.StartNew();

            string folderToIndex = project.mainFolder + "/" + project.folderToIndex;

            string[] allFolders = Directory.GetDirectories(folderToIndex, "*.*", SearchOption.AllDirectories);
            string[] displayPathes = new string[allFolders.Length];
            project.indexedFolders = new ProjectFolderData[allFolders.Length];

            int substringStartIndex = folderToIndex.Length;

            for (int i = 0; i < allFolders.Length; i++)
            {
                //displayPathes[i] = allFolders[i].Substring(substringStartIndex + 1).NormalizePath();
                string path = allFolders[i].Substring(substringStartIndex + 1).NormalizePath();
                int lastSeparatorIndex = path.LastIndexOf("/");
                displayPathes[i] = path.Substring(lastSeparatorIndex + 1) + " - " + path.Substring(0, lastSeparatorIndex == -1 ? path.Length : lastSeparatorIndex);
                project.indexedFolders[i] = new ProjectFolderData()
                {
                    path = allFolders[i],
                    displayText = displayPathes[i]
                };
            }

            Debug.Log("Indexed in " + w.ElapsedMilliseconds + "ms");
        }

        private Project TryGetProjectAt(string currentFolder)
        {
            return settings.projects.projects.FirstOrDefault(p => currentFolder.StartsWith(p.mainFolder));
        }
       
    }
}