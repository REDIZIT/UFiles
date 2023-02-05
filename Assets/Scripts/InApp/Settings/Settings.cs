using System;
using System.Collections.Generic;
using UnityEngine;

namespace InApp
{
    public class Settings
    {
        public List<FolderSortingData> folderSortingData = new List<FolderSortingData>();
        public SidebarData sidebar = new SidebarData();
        public ProjectsData projects = new ProjectsData();

        public SettingsManager manager;

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
                type = FolderSortingData.Type.BySize
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
    }

    [Serializable]
    public class Project
    {
        public string mainFolder;
        public string buildFolder;
        public List<ProjectLink> links = new List<ProjectLink>();
    }

    [Serializable]
    public class ProjectLink
    {
        public string displayText, url;
    }
}