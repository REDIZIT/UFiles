using UnityEngine;

namespace InApp.UI
{
    public class CopyEntryPathItem : ContextItem
    {
        private readonly string path;

        public CopyEntryPathItem(string path, bool isFolder)
        {
            this.path = path;
            text = "Скопировать путь до " + (isFolder ? "папки" : "файла");
        }

        public override Texture2D GetIcon()
        {
            return icons.context.copy.texture;
        }

        public override void OnClick(ContextItemEnvironment env)
        {
            GUIUtility.systemCopyBuffer = path;
        }
    }
}