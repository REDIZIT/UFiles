using TMPro;
using UnityEngine;
using Zenject;

namespace InApp.UI
{
    public class FilesViewMessager : MonoBehaviour
    {
        [SerializeField] private GameObject archiveWait;
        [SerializeField] private TextMeshProUGUI messageText;

        private float loadTime;

        [Inject] private TabUI tabs;

        private const float LOAD_TIME_TO_SHOW = 0.25f;

        public void SetMessage(string message)
        {
            messageText.gameObject.SetActive(true);
            messageText.text = message;
        }
        public void ClearMessage()
        {
            messageText.gameObject.SetActive(false);
        }
    }

}