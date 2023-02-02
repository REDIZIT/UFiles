using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace InApp.UI
{
    public class PathBar : MonoBehaviour
    {
        [SerializeField] private Transform content;
        [SerializeField] private TMP_InputField field;
        [SerializeField] private PathBarHintMaker hints;

        private List<PathBarSegment> segments = new List<PathBarSegment>();

        private FilesView view;
        private PathBarSegment.Pool pool;
        private TabUI tabs;

        [Inject]
        private void Construct(FilesView view, PathBarSegment.Pool pool, TabUI tabs)
        {
            this.view = view;
            this.pool = pool;
            this.tabs = tabs;
            view.onPathChanged += Build;
        }
        private void OnDestroy()
        {
            view.onPathChanged -= Build;
        }

        public void OnClickStartEdit()
        {
            content.gameObject.SetActive(false);
            field.gameObject.SetActive(true);
            field.Select();

            field.text = view.CurrentPath;

            hints.Show(view.CurrentPath);
        }
        public void StopEdit()
        {
            var ls = new List<RaycastResult>();
            EventSystem.current.RaycastAll(new PointerEventData(EventSystem.current)
            {
                position = Input.mousePosition
            }, ls);
            bool isClickedInside = ls.Any(r => r.gameObject.transform.IsChildOf(transform));

            if (isClickedInside == false || Input.GetKeyDown(KeyCode.Escape))
            {
                CancelFieldEdit();
            }
        }
        public void OnHintClicked(IPathBarHint hint)
        {
            if (hint != null)
            {
                field.text = hint.GetFullPath();
            }

            CancelFieldEdit();
        }

        private void CancelFieldEdit()
        {
            content.gameObject.SetActive(true);
            field.gameObject.SetActive(false);

            if (field.text != view.CurrentPath)
            {
                if (Directory.Exists(field.text))
                {
                    tabs.ActiveTab.Open(field.text);
                }
                else
                {
                    Debug.Log("Written directory doesn't exist");
                }
            }

            hints.Hide();
        }
        private void Build()
        {
            string[] pathSegments = tabs.ActiveTab.Folder.GetFullPath().Split('/');

            foreach (var segment in segments)
            {
                pool.Despawn(segment);
            }
            segments.Clear();

            
            for (int i = 0; i < pathSegments.Length; i++)
            {
                string segment = pathSegments[i];
                if (string.IsNullOrWhiteSpace(segment)) continue;

                string path = string.Join("/", pathSegments.Take(i + 1));
                if (i == 0) path += "/";

                var inst = pool.Spawn(segment, path);
                inst.transform.SetParent(content);
                inst.transform.SetAsLastSibling();
                segments.Add(inst);
            }
        }
    }
}