using TMPro;
using UnityEngine;
using Zenject;

namespace InApp.UI
{
    public class PathBarSegment : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI name;
        [SerializeField] private RectTransform rectToResize;

        private string path;
        private FilesView view;

        [Inject]
        private void Construct(FilesView view)
        {
            this.view = view;
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