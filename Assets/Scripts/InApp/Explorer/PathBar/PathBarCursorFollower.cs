using UnityEngine;
using UnityEngine.UI;

namespace InApp.UI
{
    public class PathBarCursorFollower : MonoBehaviour
    {
        [SerializeField] private float contentLeftOffset;
        [SerializeField] private float segmentLeftOffset, segmentRightOffset;
        [SerializeField] private Image image;

        private float CurrentLeft => Mathf.Lerp(startLeftX, targetLeftX, leftT);
        private float CurrentRight => Mathf.Lerp(startRightX, targetRightX, rightT);

        private float leftT, rightT;

        private RectTransform rect;
        private bool isIndexIncreased;
        private float startRightX = 40, targetRightX = 40;
        private float startLeftX, targetLeftX;
        private float t;
        private float alpha;
        private bool isShowing;

        private void Awake()
        {
            rect = GetComponent<RectTransform>();
        }
        private void Update()
        {
            if (isShowing && alpha < 1)
            {
                alpha += Time.deltaTime * 5;
            }
            if (isShowing == false && alpha > 0)
            {
                alpha -= Time.deltaTime * 5;
            }
            image.color = new Color(image.color.r, image.color.g, image.color.b, alpha);

            if (t <= 2)
            {
                t += Time.deltaTime * 10;

                if (isIndexIncreased)
                {
                    leftT = t / 2f;
                    rightT = t;
                }
                else
                {
                    leftT = t;
                    rightT = t / 2f;
                }

                SetLeft(CurrentLeft);
                SetRight(CurrentRight);
            }
        }
        public void MoveTo(RectTransform rect)
        {
            float leftX = rect.anchoredPosition.x;
            float rightX = rect.anchoredPosition.x + rect.sizeDelta.x;

            startLeftX = CurrentLeft;
            targetLeftX = leftX - segmentLeftOffset;

            startRightX = CurrentRight;
            targetRightX = rightX + segmentRightOffset;

            isIndexIncreased = CurrentRight < targetRightX;

            t = 0;
        }
        public void Show(RectTransform startRect)
        {
            if (isShowing == false)
            {
                isShowing = true;

                float leftX = startRect.anchoredPosition.x;
                float rightX = startRect.anchoredPosition.x + startRect.sizeDelta.x;

                startLeftX = targetLeftX = leftX;
                startRightX = targetRightX = rightX;
            }
        }
        public void Hide()
        {
            isShowing = false;
        }

        private void SetLeft(float x)
        {
            rect.anchoredPosition = new Vector2(Mathf.Max(contentLeftOffset + x, contentLeftOffset), 0);
        }
        private void SetRight(float x)
        {
            rect.sizeDelta = new Vector2(contentLeftOffset + x - rect.anchoredPosition.x, rect.sizeDelta.y);
        }
    }
}