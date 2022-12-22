using UnityEngine;
using Zenject;

namespace InApp.UI
{
    public class CopyFileItem : ContextItem
    {
        [Inject] private UClipboard clipboard;

        private UClipboard.CopyType type;

        public CopyFileItem(UClipboard.CopyType type)
        {
            this.type = type;
            text = type == UClipboard.CopyType.Copy ? "Копировать" : "Вырезать";
        }

        public override Texture2D GetIcon()
        {
            return type == UClipboard.CopyType.Copy ? icons.context.copy : icons.context.cut;
        }
        public override void OnClick(ContextItemEnvironment env)
        {
            clipboard.Copy(env.selectedFiles, type);
        }
    }
}