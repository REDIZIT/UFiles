using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;

namespace InApp.UI
{
    public class ContextMenuUIItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private TextMeshProUGUI text;
        [SerializeField] private GameObject arrow;
        [SerializeField] private RawImage icon;

        private ContextItem model;
        private ContextMenuUI childMenu;
        private RectTransform rect;
        private ContextMenuCreator creator;

        [Inject]
        private void Construct(ContextMenuCreator creator)
        {
            this.creator = creator;
            rect = GetComponent<RectTransform>();
        }

        private void Update()
        {
            model.Update();
            text.text = model.text;
        }
        private void Refresh(ContextItem model)
        {
            this.model = model;
            text.text = model.text;
            arrow.SetActive(model.children.Count > 0);

            icon.texture = model.GetIcon();
            icon.color = icon.texture == null ? Color.clear : Color.white;
        }

        public void OnClick()
        {
            creator.OnItemClicked(model);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (model.children.Count > 0)
            {
                childMenu = creator.ShowMenu(model.children, Vector2.zero);
                childMenu.transform.parent = transform;
                childMenu.transform.localPosition = rect.sizeDelta / 2f;
            }
        }
        public void OnPointerExit(PointerEventData eventData)
        {
            if (childMenu != null)
            {
                creator.HideMenu(childMenu);
                childMenu = null;
            }
        }

        public class Pool : MonoMemoryPool<ContextItem, ContextMenuUIItem>
        {
            protected override void Reinitialize(ContextItem p1, ContextMenuUIItem item)
            {
                item.Refresh(p1);
                base.Reinitialize(p1, item);
            }
        }
    }
}