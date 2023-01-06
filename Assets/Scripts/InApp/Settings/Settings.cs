using System;
using System.Collections.Generic;

namespace InApp
{
    public class Settings
    {
        public List<FolderSortingData> folderSortingData = new List<FolderSortingData>();
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
}