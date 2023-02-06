using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace InApp.UI
{
    public class PathBarHintMaker : MonoBehaviour
    {
        [SerializeField] private RectTransform container;
        [SerializeField] private Transform content;
        [SerializeField] private PathBarHintItem prefab;
        [SerializeField] private TMP_InputField field;
        [SerializeField] private PathBar pathBar;

        [Inject] private IconsSO icons;
        [Inject] private ProjectService projects;

        private PathBarHintItem[] pool = new PathBarHintItem[10];
        private List<IPathBarHint> staticHints = new List<IPathBarHint>();
        private List<SubFolderHint> subFolderHints = new List<SubFolderHint>();
        private int selectedHint;
        private int prevCaretPos;
        private string prevParentFolder;

        private void Awake()
        {
            for (int i = 0; i < pool.Length; i++)
            {
                pool[i] = Instantiate(prefab, content);
            }

            staticHints = GetHints().ToList();
        }
        private void Update()
        {
            UpdateKeyboard();
            UpdateHintItems();
        }
        public void Show()
        {
            container.gameObject.SetActive(true);

            Project activeProject = projects.TryGetActiveProject();
            if (activeProject != null)
            {
                projects.IndexProject(activeProject);
            }
        }
        public void Hide()
        {
            container.gameObject.SetActive(false);
        }

        public void OnFieldChanged()
        {
            
        }
        public void OnClick(IPathBarHint hint)
        {
            pathBar.OnHintClicked(hint);
        }
        private void UpdateKeyboard()
        {
            bool isArrowPressed = false;
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                isArrowPressed = true;
                selectedHint++;
            }
            else if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                isArrowPressed = true;
                selectedHint--;
            }

            selectedHint = Mathf.Clamp(selectedHint, 0, pool.Length - 1);
            
            if (isArrowPressed)
            {
                field.caretPosition = prevCaretPos;
            }
            prevCaretPos = field.caretPosition;
        }
        private void UpdateHintItems()
        {
            var sortedHints =
                staticHints.Union(GetSubFolderHints()).Union(EnumerateProjectHints())
                .Select(h => new KeyValuePair<IPathBarHint, float>(h, h.GetMatchesCount(field.text)))
                .Where(kv => kv.Value > 0)
                .OrderByDescending(kv => kv.Value);

            int hintsCount = Mathf.Min(pool.Length, sortedHints.Count());

            if (Input.GetKeyDown(KeyCode.Tab))
            {
                field.text = sortedHints.ElementAt(selectedHint).Key.GetFullPath();
                field.caretPosition = field.text.Length;
            }

            if (Input.GetKeyDown(KeyCode.Return))
            {
                if (hintsCount > 0)
                {
                    OnClick(sortedHints.ElementAt(selectedHint).Key);
                }
                else
                {
                    OnClick(null);
                }
                return;
            }


            container.sizeDelta = new Vector2(container.sizeDelta.x, 29 + 24 * hintsCount);
            selectedHint = Mathf.Clamp(selectedHint, 0, hintsCount - 1);


            int poolIndex = 0;
            foreach (var kv in sortedHints)
            {
                if (poolIndex >= pool.Length)
                {
                    break;
                }

                PathBarHintItem item = pool[poolIndex];
                item.gameObject.SetActive(true);
                item.Refresh(kv.Key, field.text, this, poolIndex == selectedHint);

                poolIndex++;
            }

            for (int i = poolIndex; i < pool.Length; i++)
            {
                pool[poolIndex].gameObject.SetActive(false);
            }
        }
        private IEnumerable<IPathBarHint> GetHints()
        {
            string userFolder = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            yield return GetFolderHint(userFolder);
            yield return GetFolderHint(userFolder + "/AppData");
            yield return GetFolderHint(userFolder + "/AppData/Roaming");
            yield return GetFolderHint(userFolder + "/AppData/LocalLow");
            yield return GetFolderHint(userFolder + "/AppData/Local");
            yield return GetFolderHint(userFolder + "/Downloads");
            yield return GetFolderHint(userFolder + "/Videos");
        }
        private LocalFolderHint GetFolderHint(string folder)
        {
            folder = folder.Replace(@"\", "/");
            return new LocalFolderHint(icons, folder, Path.GetFileName(folder));
        }
        private IEnumerable<IPathBarHint> GetSubFolderHints()
        {
            int index = field.text.LastIndexOf("/");

            if (index > 0)
            {
                string inputtedParentFolder = field.text.Substring(0, index + 1);

                if (prevParentFolder != inputtedParentFolder && Directory.Exists(inputtedParentFolder))
                {
                    prevParentFolder = inputtedParentFolder;
                    subFolderHints.Clear();
                    var folders = Directory.EnumerateDirectories(inputtedParentFolder);
                    foreach (string folder in folders)
                    {
                        var hint = new SubFolderHint(inputtedParentFolder, Path.GetFileName(folder), icons);
                        subFolderHints.Add(hint);
                        yield return hint;
                    }
                }
                else
                {
                    foreach (SubFolderHint hint in subFolderHints)
                    {
                        yield return hint;
                    }
                }
            }
        }
        private IEnumerable<IPathBarHint> EnumerateProjectHints()
        {
            // Enumerate all projects
            foreach (Project project in projects.EnumerateProjects())
            {
                yield return new ProjectHint(icons, project);
            }

            // Enumerate active project indexed folders
            Project activeProject = projects.TryGetActiveProject();
            if (activeProject != null && activeProject.isIndexing == false)
            {
                foreach (ProjectFolderData data in activeProject.indexedFolders)
                {
                    yield return new ProjectFolderHint(data, icons);
                }
            }
        }
    }
}