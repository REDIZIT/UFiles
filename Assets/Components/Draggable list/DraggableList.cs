using UnityEngine;

namespace InApp.DraggableList
{
    public class DraggableList : MonoBehaviour
    {
        [SerializeField] private Transform content, dragContent;
        [SerializeField] private Transform dragPlaceholder;

        [SerializeField] private DraggableElement prefab;
        
        private DraggableElement draggingItem;
        private int startIndex;

        private void Start()
        {
            for (int i = 0; i < 20; i++)
            {
                var inst = Instantiate(prefab, content);
                inst.index = i;
                inst.list = this;
            }
        }
        private void Update()
        {
            if (draggingItem != null)
            {
                draggingItem.transform.position = Input.mousePosition;

                float deltaY = draggingItem.transform.position.y - dragPlaceholder.position.y;
                float yPerElement = 24 + 2;

                int index = startIndex + (int)(deltaY / yPerElement);

                dragPlaceholder.transform.SetSiblingIndex(index);

                if (Input.GetMouseButtonUp(0))
                {
                    StopDrag();
                }
            }
        }
        public void OnDragBegin(DraggableElement element)
        {
            element.transform.parent = dragContent;
            dragPlaceholder.gameObject.SetActive(true);
            dragPlaceholder.SetSiblingIndex(element.index);
            startIndex = element.index;
            draggingItem = element;
        }
        private void StopDrag()
        {
            int index = dragPlaceholder.GetSiblingIndex();
            dragPlaceholder.gameObject.SetActive(false);
            draggingItem.transform.parent = content;
            draggingItem.transform.SetSiblingIndex(index);
            draggingItem = null;
        }
    }
}