using UnityEngine;

namespace InApp.DraggableList
{
    public class DraggableElement : MonoBehaviour
    {
        public int index;
        public DraggableList list;

        [SerializeField] private Transform ui;

        private RectTransform rect;

        private void Awake()
        {
            rect = GetComponent<RectTransform>();
        }
        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (RectTransformUtility.RectangleContainsScreenPoint(rect, Input.mousePosition))
                {
                    list.OnDragBegin(this);
                }
            }
        }
    }
}