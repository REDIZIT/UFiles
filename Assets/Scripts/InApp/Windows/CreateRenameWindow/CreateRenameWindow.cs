using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace InApp.UI
{
    public class CreateRenameWindow : Window<CreateRenameWindow.Model>
    {
        [SerializeField] private TextMeshProUGUI label;
        [SerializeField] private TMP_InputField nameField;
        [SerializeField] private GameObject existsGroup;
        [SerializeField] private Button applyButton;

        public class Model
        {
            public ContextItemEnvironment env;
            public string title;
            public EntryType entryType;
            public string filename;
            public Action<string> onApply;
        }

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
        protected override void OnShowed()
        {
            label.text = model.title;
            nameField.text = model.filename;
            nameField.Select();
        }
        public void OnClickCancel()
        {
            Close();
        }
        public void OnClickApply()
        {
            if (IsValid())
            {
                model.onApply?.Invoke(nameField.text);
                Close();
            }
        }
        private bool IsValid()
        {
            string text = nameField.text.Trim();
            if (string.IsNullOrWhiteSpace(text)) return false;

            text = model.env.currentFolder + "/" + text;

            return EntryUtils.Exists(text) == false;
        }
    }
}