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
        private Action onCancel;
        private Action<string> onApply;


        private void Update()
        {
            bool exists = File.Exists(env.currentFolder + "/" + nameField.text);
            applyButton.interactable = exists == false;
            existsGroup.SetActive(exists);

            if (Input.GetKeyDown(KeyCode.Return) && exists == false)
            {
                OnClickApply();
            }
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                OnClickCancel();
            }
        }
        public void Show(ContextItemEnvironment env, string filename, Action onCancel, Action<string> onApply)
        {
            this.env = env;
            this.onCancel = onCancel;
            this.onApply = onApply;

            gameObject.SetActive(true);

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
            onApply?.Invoke(nameField.text);
            Close();
        }
    }
}