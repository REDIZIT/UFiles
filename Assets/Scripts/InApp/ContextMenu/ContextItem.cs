using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace InApp.UI
{
    public class ContextItem
    {
        public string text;
        public List<ContextItem> children = new();

        [Inject] protected IconsSO icons;

        public virtual Texture2D GetIcon() { return null; }
        public virtual void Update() { }
        public virtual void OnClick(ContextItemEnvironment env) { }
    }
    public class ContextItemEnvironment
    {
        public string currentFolder;
        public string selectedEntryName;
        public IEnumerable<string> selectedFiles;
    }
    public class RenameItem : ContextItem
    {
        public override void OnClick(ContextItemEnvironment env)
        {
            Debug.Log("Rename " + env.selectedEntryName);
        }
    }
}