using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace InApp
{
    public class AppDragDrop : MonoBehaviour
    {
        public bool IsDragging => holdingItem != null;

        [SerializeField] private RectTransform tooltip;
        [SerializeField] private Image icon;
        [SerializeField] private TextMeshProUGUI text;

        [SerializeField] private RectTransform itemIconPlaceholder;
        [SerializeField] private Image itemIcon;
        [SerializeField] private TextMeshProUGUI itemIconText;

        private DragDropData holdingItem;
        private AppDragDropTarget activeTarget;

        private List<RaycastResult> results = new List<RaycastResult>();

        public void StartDrag(DragDropData item)
        {
            holdingItem = item;
        }

        private void Update()
        {
            if (holdingItem != null)
            {
                EventSystem.current.RaycastAll(new PointerEventData(EventSystem.current)
                {
                    position = Input.mousePosition
                }, results);

                AppDragDropTarget target = null;
                foreach (var item in results)
                {
                    target = item.gameObject.GetComponent<AppDragDropTarget>();
                    if (target != null) break;
                }

                if (activeTarget != null && activeTarget != target)
                {
                    activeTarget.OnMouseExit();
                }


                bool targetWillTake = target != null && target.WillHandleDrop(holdingItem);

                if (targetWillTake)
                {
                    target.OnMouseOver(holdingItem);
                }
                
                if (Input.GetMouseButtonUp(0))
                {
                    if (targetWillTake)
                    {
                        target.OnDrop(holdingItem);
                        target.OnMouseExit();
                    }

                    holdingItem = null;
                }

                activeTarget = target;
            }

            UpdateTooltip();
            UpdateIcon();
        }

        private void UpdateTooltip()
        {
            if (activeTarget == null || holdingItem == null || activeTarget.WillHandleDrop(holdingItem) == false)
            {
                tooltip.gameObject.SetActive(false);
            }
            else
            {
                tooltip.gameObject.SetActive(true);
                tooltip.anchoredPosition = Input.mousePosition;
                icon.sprite = activeTarget.GetIcon();
                text.text = activeTarget.GetDisplayText();
            }
        }
        private void UpdateIcon()
        {
            if (holdingItem == null)
            {
                itemIconPlaceholder.gameObject.SetActive(false);
            }
            else
            {
                itemIconPlaceholder.gameObject.SetActive(true);
                itemIconPlaceholder.anchoredPosition = Input.mousePosition;
                itemIcon.sprite = holdingItem.GetIcon();
                itemIconText.text = holdingItem.GetDisplayText();
            }
        }
    }

    public interface AppDragDropTarget
    {
        Sprite GetIcon();
        string GetDisplayText();
        /// <summary>
        /// Return true if this target will handle drop events, and return false to ignore this drag data item
        /// </summary>
        bool WillHandleDrop(DragDropData data);
        void OnMouseOver(DragDropData data);
        void OnMouseExit();
        void OnDrop(DragDropData data);
    }
    public abstract class DragDropData
    {
        public abstract Sprite GetIcon();
        public abstract string GetDisplayText();
    }
}