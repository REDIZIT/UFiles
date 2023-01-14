using Stopwatch = System.Diagnostics.Stopwatch;
using System.Drawing;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace InApp
{
    public class PicturePreview : MonoBehaviour
    {
        [SerializeField] private CanvasGroup group;
        [SerializeField] private RawImage image;
        [SerializeField] private TextMeshProUGUI infoText;
        [SerializeField] private RectTransform arrow;

        private RectTransform rect, sidebarRect;
        private Texture2D texture;

        private readonly string[] extensions = new string[] { ".png", ".jpg" };

        private void Start()
        {
            rect = GetComponent<RectTransform>();
            sidebarRect = rect.parent.GetComponent<RectTransform>();
        }
        public void TryOpen(string filepath, RectTransform rectAlignTo)
        {
            string ext = Path.GetExtension(filepath);

            if (extensions.Contains(ext))
            {
                group.alpha = 1;

                byte[] bytes = File.ReadAllBytes(filepath);

                if (texture == null)
                {
                    texture = new Texture2D(1, 1);
                }

                texture.filterMode = Mathf.Min(texture.width, texture.height) <= 128 ? FilterMode.Point : FilterMode.Bilinear;
                texture.LoadImage(bytes);
                image.texture = texture;


                infoText.text = texture.width + "x" + texture.height;

                float ratio = texture.height / (float)texture.width;
                float height = rect.rect.width * ratio;

                rect.sizeDelta = new Vector2(rect.sizeDelta.x, height);


                float sidebarMax = sidebarRect.anchorMax.y * Screen.height + sidebarRect.sizeDelta.y - 12;
                float sidebarMin = sidebarRect.anchorMin.y * Screen.height + 28 + 12;

                sidebarMax -= height / 2f;
                sidebarMin += height / 2f;

                float y = rectAlignTo.TransformPoint(rectAlignTo.rect.center).y;
                float windowY = Mathf.Clamp(y, sidebarMin, sidebarMax);

                rect.anchoredPosition = new Vector2(rect.anchoredPosition.x, windowY);


                float diff = windowY - y;
                diff = Mathf.Clamp(diff, -height / 2f + 10, height / 2f - 10);
                arrow.anchoredPosition = new Vector2(rect.anchoredPosition.x, -diff);
            }
        }
        public void Hide()
        {
            group.alpha = 0;
        }
    }
}