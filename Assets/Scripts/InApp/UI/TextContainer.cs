using TMPro;
using UnityEngine;

namespace InApp.UI
{
    [ExecuteAlways]
    public class TextContainer : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI text;
        [SerializeField] private Vector2 padding;

        private RectTransform rect;

        private void Awake()
        {
            rect = GetComponent<RectTransform>();
        }

        private void LateUpdate()
        {
            if (text == null) return;
            rect.sizeDelta = new Vector2(text.preferredWidth + padding.x, text.preferredHeight + padding.y);
        }
    }
}