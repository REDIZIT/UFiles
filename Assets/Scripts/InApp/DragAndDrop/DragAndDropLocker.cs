using Stampcrawler;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace InApp
{
    public class DragAndDropLocker : MonoBehaviour
    {
        [SerializeField] private RawImage image;

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                //DragAndDrop.Start("C:/1.txt");
                //new ClipboardTex

                //var text = new ClipboardTexture();
                //image.texture = text.GetClipboardTexture();

                var tex = ClipboardImage.Copy();
                image.texture = tex;
            }
        }
    }
}