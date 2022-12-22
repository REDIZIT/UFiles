using System;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace InApp.UI
{
    public class CreateRenameWindow : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI label;
        [SerializeField] private TMP_InputField nameField;
        [SerializeField] private GameObject existsGroup;
        [SerializeField] private Button applyButton;

        private ContextItemEnvironment env;
        private bool createDirectory;
        private Action onCancel;
        private Action<string> onApply;

        private void Update()
        {
            bool isValid = IsValid();
            applyButton.interactable = isValid;
            existsGroup.SetActive(isValid == false);

            if (Input.GetKeyDown(KeyCode.Return) && isValid)
            {
                OnClickApply();
            }
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                OnClickCancel();
            }
        }
        public void Show(ContextItemEnvironment env, bool createDirectory, string filename, Action onCancel, Action<string> onApply)
        {
            this.env = env;
            this.createDirectory = createDirectory;
            this.onCancel = onCancel;
            this.onApply = onApply;

            gameObject.SetActive(true);

            label.text = createDirectory ? "Создать папку" : "Создать файл";
            nameField.text = filename;
            nameField.Select();
        }
        public void Close()
        {
            gameObject.SetActive(false);
        }
        public void OnClickCancel()
        {
            onCancel?.Invoke();
            Close();
        }
        public void OnClickApply()
        {
            if (IsValid())
            {
                onApply?.Invoke(nameField.text);
                Close();
            }
        }
        private bool IsValid()
        {
            string text = nameField.text.Trim();
            if (string.IsNullOrWhiteSpace(text)) return false;

            text = env.currentFolder + "/" + text;

            if (createDirectory)
            {
                return Directory.Exists(text) == false;
            }
            else
            {
                return File.Exists(text) == false;
            }
        }
    }
}