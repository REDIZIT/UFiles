using TMPro;
using UnityEngine;
using Zenject;

namespace InApp.UI
{
    public class ContextMenuUIItem : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI text;

        private ContextItem model;
        private ContextMenuCreator menu;

        [Inject]
        private void Construct(ContextMenuCreator menu)
        {
            this.menu = menu;
        }

        private void Update()
        {
            model.Update();
            text.text = model.text;
        }

        public void OnClick()
        {
            menu.OnItemClicked(model);
        }

        public class Pool : MonoMemoryPool<ContextItem, ContextMenuUIItem>
        {
            protected override void Reinitialize(ContextItem p1, ContextMenuUIItem item)
            {
                item.text.text = p1.text;
                item.model = p1;

                base.Reinitialize(p1, item);
            }
        }
    }
}