using UnityEngine;
using Zenject;

namespace InApp
{
    public class ResizeArea : MonoBehaviour
    {
        private bool handleClicked;
        private Vector2 clickedPosition;
        private Rect originalWindow, fileWindow;
        private RectTransform rect;

        private void Awake()
        {
            rect = GetComponent<RectTransform>();
        }

        private void Update()
        {
            var mousePos = Input.mousePosition;
            mousePos.y = Screen.height - mousePos.y;    // Convert to GUI coords
            //Rect windowHandle = Rect(fileWindow.x + fileWindow.width - 25, fileWindow.y + fileWindow.height - 25, 25, 25);

            // If clicked on window resize widget
            if (Input.GetMouseButtonDown(0) /*&& RectTransformUtility.RectangleContainsScreenPoint(rect, mousePos)*/) {
                handleClicked = true;
                clickedPosition = mousePos;
                originalWindow = fileWindow;
                Debug.Log("Clicked");
            }

            if (handleClicked)
            {
                // Resize window by dragging
                if (Input.GetMouseButton(0))
                {
                    Debug.Log("Drag");
                    fileWindow.width = Mathf.Clamp(originalWindow.width + (mousePos.x - clickedPosition.x), 10, 1000);
                    fileWindow.height = Mathf.Clamp(originalWindow.height + (mousePos.y - clickedPosition.y), 10, 1000);

                    rect.sizeDelta = new Vector2(fileWindow.width, fileWindow.height);
                }
                // Finish resizing window
                if (Input.GetMouseButtonUp(0))
                {
                    Debug.Log("Release");
                    handleClicked = false;
                }
            }
        }
    }
}