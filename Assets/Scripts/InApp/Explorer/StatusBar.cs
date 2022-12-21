using TMPro;
using UnityEngine;
using Zenject;

namespace InApp.UI
{
    public class StatusBar : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI countText, selectedCountText;

        private FilesView files;

        [Inject]
        private void Construct(FilesView files)
        {
            this.files = files;
        }

        private void Update()
        {
            countText.text = "Элементов: " + files.EntriesCount;
            int selectedCount = files.SelectedEntriesCount;

            if (selectedCount > 0)
            {
                selectedCountText.text = "Выбрано: " + selectedCount;
            }
            else
            {
                selectedCountText.text = string.Empty;
            }
        }
    }
}