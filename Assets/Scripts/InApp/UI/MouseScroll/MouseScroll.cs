using System;
using UnityEngine;
using UnityEngine.UI;

namespace InApp.UI
{
    public class MouseScroll : MonoBehaviour
    {
        public bool IsScrolling { get; private set; }

        [SerializeField] private Transform pivot;
        [SerializeField] private RectTransform topArrow, bottomArrow;
        [SerializeField] private Color defaultColor;
        
        private Image topArrowImage, bottomArrowImage;
        private Action<float> deltaCallback;

        private void Awake()
        {
            topArrowImage = topArrow.GetComponent<Image>();
            bottomArrowImage = bottomArrow.GetComponent<Image>();
        }
        private void Update()
        {
            if (IsScrolling == false) return;

            float delta = Input.mousePosition.y - pivot.position.y;

            float visualDelta = delta / 3f;
            float topDelta = Mathf.Max(visualDelta, 0) + 6;
            float bottomDelta = Mathf.Min(visualDelta, 0) - 5;

            topArrow.anchoredPosition = new Vector2(.1f, topDelta);
            bottomArrow.anchoredPosition = new Vector2(.1f, bottomDelta);

            topArrowImage.color = defaultColor.SetTransparency(delta / 10f + 2);
            bottomArrowImage.color = defaultColor.SetTransparency(-delta / 10f + 2);

            deltaCallback?.Invoke(delta);

            if (Input.GetMouseButtonUp(2))
            {
                IsScrolling = false;
                pivot.gameObject.SetActive(false);
            }
        }
        public void Begin(Action<float> deltaCallback)
        {
            pivot.gameObject.SetActive(true);
            pivot.transform.position = Input.mousePosition;

            IsScrolling = true;
            this.deltaCallback = deltaCallback;
        }
    }
}