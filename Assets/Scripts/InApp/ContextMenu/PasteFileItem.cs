using UnityEngine;
using Zenject;

namespace InApp.UI
{
    public class PasteFileItem : ContextItem
    {
        [Inject] private UClipboard clipboard;

        public PasteFileItem()
        {
            text = "Вставить";
        }

        public override Texture2D GetIcon()
        {
            return icons.context.paste.texture;
        }

        public override void OnClick(ContextItemEnvironment env)
        {
            clipboard.Paste(env.currentFolder);
        }
    }
}