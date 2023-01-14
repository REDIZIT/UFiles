using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace InApp.UI
{
    public class PointerHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public RectTransform Rect { get; private set; }

        public Action onPointerEnter, onPointerExit;

        private void Awake()
        {
            Rect = GetComponent<RectTransform>();
        }
        public void OnPointerEnter(PointerEventData eventData)
        {
            onPointerEnter?.Invoke();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            onPointerExit?.Invoke();
        }
    }
}