using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace InApp.UI
{
    public class PathBarSegment : MonoBehaviour, IDragHandler, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private TextMeshProUGUI name;
        [SerializeField] private RectTransform rectToResize;

        private string path;
        private TabUI tabs;
        private AppDragDrop dragDrop;
        private IconsSO icons;
        private PathBar bar;

        private RectTransform rect;

        [Inject]
        private void Construct(TabUI tabs, AppDragDrop dragDrop, IconsSO icons, PathBar bar)
        {
            this.tabs = tabs;
            this.dragDrop = dragDrop;
            this.icons = icons;
            this.bar = bar;

            rect = GetComponent<RectTransform>();
        }

        public void Refresh(string segment, string path)
        {
            this.path = path;
            name.text = segment;
            rectToResize.sizeDelta = new Vector2(name.preferredWidth + 14, rectToResize.sizeDelta.y);
        }
        public void OnClick()
        {
            tabs.ActiveTab.Open(path);
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (dragDrop.IsDragging) return;

            dragDrop.StartDrag(new EntryUIItem.EntryData()
            {
                icon = icons.folderFill,
                path = path,
            });
        }
        public void OnPointerEnter(PointerEventData eventData)
        {
            bar.OnSegmentEnter(rect);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            bar.OnSegmentExit();
        }

        public class Pool : MemoryPool<string, string, PathBarSegment>
        {
            protected override void OnDespawned(PathBarSegment item)
            {
                item.gameObject.SetActive(false);
            }
            protected override void Reinitialize(string segment, string path, PathBarSegment item)
            {
                item.gameObject.SetActive(true);
                item.Refresh(segment, path);
            }
        }
    }
}