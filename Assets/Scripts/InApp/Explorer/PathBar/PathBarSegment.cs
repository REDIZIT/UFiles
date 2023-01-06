using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace InApp.UI
{
    public class PathBarSegment : MonoBehaviour, IDragHandler
    {
        [SerializeField] private TextMeshProUGUI name;
        [SerializeField] private RectTransform rectToResize;

        private string path;
        private FilesView view;
        private AppDragDrop dragDrop;
        private IconsSO icons;

        [Inject]
        private void Construct(FilesView view, AppDragDrop dragDrop, IconsSO icons)
        {
            this.view = view;
            this.dragDrop = dragDrop;
            this.icons = icons;
        }

        public void Refresh(string segment, string path)
        {
            this.path = path;
            name.text = segment;
            rectToResize.sizeDelta = new Vector2(name.preferredWidth + 10, rectToResize.sizeDelta.y);
        }
        public void OnClick()
        {
            view.Show(path);
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