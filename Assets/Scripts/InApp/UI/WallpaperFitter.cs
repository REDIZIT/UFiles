using UnityEngine;
using UnityEngine.UI;

namespace InApp.UI
{
    [ExecuteAlways]
    public class WallpaperFitter : MonoBehaviour
    {
        [SerializeField] private Image wallpaper;

        [SerializeField] private float verticalOffset;

        private RectTransform rect;

        private void Awake()
        {
            rect = wallpaper.rectTransform;
        }
        private void Update()
        {
            float screenRatio = Screen.width / (float)Screen.height;

            float imageRatio = wallpaper.sprite.bounds.size.y / wallpaper.sprite.bounds.size.x;

            float height = Screen.width * imageRatio;
            if (height < Screen.height)
            {
                wallpaper.rectTransform.sizeDelta = new Vector2(Screen.height / imageRatio, Screen.height);
            }
            else
            {
                wallpaper.rectTransform.sizeDelta = new Vector2(Screen.width, height);
            }
        }
    }
}