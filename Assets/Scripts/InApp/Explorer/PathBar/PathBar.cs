using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace InApp.UI
{
    public class PathBar : MonoBehaviour, IPointerExitHandler
    {
        [SerializeField] private Transform content;
        [SerializeField] private TMP_InputField field;
        [SerializeField] private PathBarHintMaker hints;
        [SerializeField] private PathBarCursorFollower follower;

        private List<PathBarSegment> segments = new List<PathBarSegment>();
        private int followerHideFramesToSkip = -1;

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
        private void Update()
        {
            if (field.gameObject.activeSelf)
            {
                if (Input.GetKeyDown(KeyCode.Backspace) && Input.GetKey(KeyCode.LeftControl))
                {
                    for (int i = field.caretPosition - 1; i >= 0; i--)
                    {
                        if (field.text[i] == '/')
                        {
                            field.text = field.text.Substring(0, i + 1);
                            break;
                        }
                    }
                }
            }

            if (followerHideFramesToSkip > 0)
            {
                followerHideFramesToSkip--;
            }
            else if (followerHideFramesToSkip == 0)
            {
                followerHideFramesToSkip = -1;
                follower.Hide();
            }
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

            hints.Show();
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
        public void OnSegmentEnter(RectTransform rect)
        {
            follower.Show(rect);
            follower.MoveTo(rect);

            followerHideFramesToSkip = -1;
        }
        public void OnSegmentExit()
        {
            followerHideFramesToSkip = 1;
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
                if (i != 0) path += "/";

                var inst = pool.Spawn(segment, path);
                inst.transform.SetParent(content);
                inst.transform.SetAsLastSibling();
                segments.Add(inst);
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            follower.Hide();
        }
    }
}