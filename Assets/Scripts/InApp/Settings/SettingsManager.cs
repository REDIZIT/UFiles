using System.IO;
using UnityEngine;
using Zenject;

namespace InApp
{
    public class SettingsManager
    {
#if UNITY_EDITOR
        private const string FOLDER_PATH = "/../Build/Data/";
#elif UNITY_STANDALONE
        private const string FOLDER_PATH = "/../Data/";
#endif
        private const string FILE_NAME = "settings.json";

        public void InstallBindings(DiContainer container)
        {
            string filepath = Application.dataPath + FOLDER_PATH + FILE_NAME;

            Settings settings;

            if (File.Exists(filepath))
            {
                string json = File.ReadAllText(filepath);
                settings = JsonUtility.FromJson<Settings>(json);
                settings.manager = this;
            }
            else
            {
                settings = Settings.CreateNew(this);
                settings.Save();
            }

            container.BindInstance(settings);
        }

        public void Save(Settings settings)
        {
            string json = JsonUtility.ToJson(settings, true);
            Directory.CreateDirectory(Application.dataPath + FOLDER_PATH);
            File.WriteAllText(Application.dataPath + FOLDER_PATH + FILE_NAME, json);
        }
    }
}