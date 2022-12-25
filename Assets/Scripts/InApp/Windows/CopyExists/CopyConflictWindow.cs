using System;
using System.IO;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace InApp.UI
{
    public class CopyConflictWindow : DialogWindow<CopyConflictWindow.Model, CopyConflictWindow.Answer>
    {
        [SerializeField] private RawImage image;
        [SerializeField] private TextMeshProUGUI nameText, folderA, folderB;

        private Answer answer;
        private FilePreview preview;

        [Inject] private IconsSO icons;

        public class Model
        {
            public string filepath;
            public string sourceFolder, targetFolder;
            public Action onOverwriteClicked, onRenameClicked, onCancelClicked;
        }
        public enum Answer
        {
            None,
            Cancel,
            Overwrite,
            Rename,
        }

        protected override void OnAwake()
        {
            base.OnAwake();

            preview = new FilePreview(image);
        }
        protected override void OnShowed()
        {
            base.OnShowed();

            nameText.text = Path.GetFileName(model.filepath);
            folderA.text = model.sourceFolder;
            folderB.text = model.targetFolder;
            answer = Answer.None;

            if (preview.CanHandle(model.filepath))
            {
                preview.Load(model.filepath);
            }
            else
            {
                image.texture = EntryUtils.GetType(model.filepath) == EntryType.Directory ? icons.folderOpen : icons.defaultFile;
            }
        }
        protected override async Task<Answer> GetAnswer()
        {
            while (answer == Answer.None)
            {
                await Task.Yield();
            }

            Close();
            return answer;
        }
        protected override void OnUpdate()
        {
            base.OnUpdate();

            if (Input.GetKeyDown(KeyCode.Return))
            {
                OnOverwriteClicked();
            }
            else if (Input.GetKeyDown(KeyCode.Escape))
            {
                OnCancelClicked();
            }
        }

        public void OnOverwriteClicked()
        {
            answer = Answer.Overwrite;
            model.onOverwriteClicked?.Invoke();
        }
        public void OnRenameClicked()
        {
            answer = Answer.Rename;
            model.onRenameClicked?.Invoke();
        }
        public void OnCancelClicked()
        {
            answer = Answer.Cancel;
            model.onCancelClicked?.Invoke();
        }
    }
}