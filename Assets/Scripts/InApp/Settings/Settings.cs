using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Zenject;

namespace InApp
{
    public class Settings
    {
        public List<FolderSortingData> folderSortingData = new List<FolderSortingData>();
        public SidebarData sidebar = new SidebarData();
        public ProjectsData projects = new ProjectsData();

        public SettingsManager manager;

        [Inject]
        private void Construct(DiContainer container)
        {
            container.Inject(projects);
        }

        private Settings(SettingsManager manager)
        {
            this.manager = manager;
        }

        public static Settings CreateNew(SettingsManager manager)
        {
            Settings settings = new Settings(manager);

            settings.folderSortingData.Add(new FolderSortingData()
            {
                path = "C:/Users/redizit/Downloads",
                type = FolderSortingData.Type.ByDate
            });

            settings.sidebar = new SidebarData()
            {
                favourite = new List<string>()
                {
                    "C:/Users/redizit/Downloads"
                }
            };

            return settings;
        }

        public void Save()
        {
            manager.Save(this);
        }
    }


    [Serializable]
    public class FolderSortingData
    {
        public string path;
        public Type type;
        public bool isReversed;

        public enum Type
        {
            None,
            /// <summary>
            /// By default from A to Z
            /// </summary>
            ByName,
            /// <summary>
            /// By default from new to old
            /// </summary>
            ByDate,
            /// <summary>
            /// By default from big to small
            /// </summary>
            BySize
        }
    }

    [Serializable]
    public class SidebarData
    {
        public List<string> favourite = new List<string>();
    }

    [Serializable]
    public class ProjectsData
    {
        public List<Project> projects = new List<Project>();

        [Inject]
        private void Construct(DiContainer container)
        {
            foreach (Project project in projects)
            {
                container.Inject(project);
            }
        }
    }

    [Serializable]
    public class Project
    {
        public string mainFolder;
        public string buildExecutable = "Build";
        public string folderToIndex = "Assets";

        public List<ProjectLink> links = new List<ProjectLink>();

        [NonSerialized]
        public ProjectFolderData[] indexedFolders = new ProjectFolderData[0];
        public bool isIndexing;
    }

    [Serializable]
    public class ProjectLink
    {
        public string displayText, url;
        public string iconID;
    }

    public class ProjectFolderData
    {
        public string path;
        public string displayText;
        public int depth;
    }
}