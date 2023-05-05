using System.IO;
using UnityEngine;
using Zenject;

namespace InApp
{
    public class SettingsManager
    {
        private const string FILE_NAME = "settings.json";

        public void InstallBindings(DiContainer container)
        {
            string filepath = Application.persistentDataPath + "/" + FILE_NAME;

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
            Directory.CreateDirectory(Application.persistentDataPath + "/");
            File.WriteAllText(Application.persistentDataPath + "/" + FILE_NAME, json);
        }
    }
}