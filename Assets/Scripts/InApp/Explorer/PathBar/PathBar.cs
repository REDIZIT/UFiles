using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using Zenject;

namespace InApp.UI
{
    public class PathBar : MonoBehaviour
    {
        [SerializeField] private Transform content;
        [SerializeField] private TMP_InputField field;

        private List<PathBarSegment> segments = new List<PathBarSegment>();

        private FilesView view;
        private PathBarSegment.Pool pool;

        [Inject]
        private void Construct(FilesView view, PathBarSegment.Pool pool)
        {
            this.view = view;
            this.pool = pool;
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
        }
        public void StopEdit()
        {
            content.gameObject.SetActive(true);
            field.gameObject.SetActive(false);

            if (field.text != view.CurrentPath)
            {
                if (Directory.Exists(field.text))
                {
                    view.Show(field.text);
                }
                else
                {
                    Debug.Log("Written directory doesn't exist");
                }
            }
        }

        private List<PathBarSegment> GetSegments()
        {
            return segments;
        }

        private void Build()
        {
            string[] pathSegments = view.CurrentPath.Split('/');

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
                inst.transform.parent = content;
                inst.transform.SetAsLastSibling();
                segments.Add(inst);
            }
        }
    }
}