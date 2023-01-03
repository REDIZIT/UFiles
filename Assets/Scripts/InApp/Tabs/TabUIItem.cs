using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace InApp
{
    public class Tab
    {
        public IPath path;
    }
    public class TabUIItem : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI text;
        [SerializeField] private Button image;

        [SerializeField] private Color activeColor, activeHighlighted;

        private RectTransform rect;
        private ColorBlock defaultBlock, activeBlock;
        private Tab model;

        private TabUI tabs;
        private Pool pool;

        [Inject]
        private void Construct(TabUI tabs, Pool pool)
        {
            this.tabs = tabs;
            this.pool = pool;

            rect = GetComponent<RectTransform>();

            defaultBlock = image.colors;
            activeBlock = defaultBlock;
            activeBlock.normalColor = activeColor;
            activeBlock.highlightedColor = activeHighlighted;
        }

        private void Update()
        {
            bool isActive = model == tabs.ActiveTab;
            text.text = model.path.GetDisplayName();

            image.colors = isActive ? activeBlock : defaultBlock;

            if (Input.GetMouseButtonDown(2) && RectTransformUtility.RectangleContainsScreenPoint(rect, Input.mousePosition))
            {
                pool.Despawn(this);
                tabs.OnCloseTabClicked(model);
            }

            FitSize();
        }
        private void OnEnable()
        {
            tabs.onActiveTabChanged += Update;
        }
        private void OnDisable()
        {
            tabs.onActiveTabChanged -= Update;
        }

        public void OnClick()
        {
            tabs.OnTabClicked(model);
        }
        private void FitSize()
        {
            rect.sizeDelta = new Vector2(tabs.TabWidth, rect.sizeDelta.y);
        }


        public class Pool : MonoMemoryPool<Tab, TabUIItem>
        {
            protected override void Reinitialize(Tab p1, TabUIItem item)
            {
                base.Reinitialize(p1, item);
                item.model = p1;
            }
        }
    }
}