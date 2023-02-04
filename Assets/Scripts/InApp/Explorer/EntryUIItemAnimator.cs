using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace InApp.UI
{
    public class EntryUIItemAnimator : MonoBehaviour
    {
        [Range(0, 1)]
        [SerializeField] private float progress;
        [SerializeField] private RawImage image;
        [SerializeField] private TextMeshProUGUI name, size, time;

        private RectTransform imageRect, nameRect;
        private bool isPlaying;

        private void OnEnable()
        {
            imageRect = image.GetComponent<RectTransform>();
            nameRect = name.GetComponent<RectTransform>();
        }
        private void Update()
        {
            if (isPlaying)
            {
                progress = Mathf.Clamp01(progress + Time.deltaTime * 6);
            }

            EvaluateAnimation();
        }
        public void Show()
        {
            isPlaying = true;
            progress = 0;

            EvaluateAnimation();
        }
        public void ResetAnimation()
        {
            isPlaying = false;
            progress = 0;

            EvaluateAnimation();
        }

        private void EvaluateAnimation()
        {
            Color color = new Color(1, 1, 1, progress);
            image.color = color;
            name.color = color;
            size.color = color;
            time.color = color;

            imageRect.anchoredPosition = new Vector2(Mathf.Lerp(18 + 6, 18, progress), 0);
            nameRect.anchoredPosition = new Vector2(Mathf.Lerp(42 + 8, 42, progress), 0);
        }
    }
}