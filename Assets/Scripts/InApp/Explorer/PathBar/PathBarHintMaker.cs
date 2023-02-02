using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace InApp.UI
{
    public class PathBarHintMaker : MonoBehaviour
    {
        [SerializeField] private RectTransform container;
        [SerializeField] private Transform content;
        [SerializeField] private PathBarHintItem prefab;
        [SerializeField] private TMP_InputField field;
        [SerializeField] private PathBar pathBar;

        private PathBarHintItem[] pool = new PathBarHintItem[10];
        private bool isShowed;
        private string startFolder;
        private List<IPathBarHint> hints = new List<IPathBarHint>();
        private int selectedHint;
        private int prevCaretPos;

        //private void Awake()
        //{
        //    for (int i = 0; i < pool.Length; i++)
        //    {
        //        pool[i] = Instantiate(prefab, content);
        //    }
        //}
        //private void Update()
        //{
        //    if (isShowed == false) return;

        //    UpdateKeyboard();
        //    UpdateHintItems();
        //}
        public void Show(string startFolder)
        {
            //container.gameObject.SetActive(true);
            //isShowed = true;
            //this.startFolder = startFolder;

            //hints = GetHints().ToList();
        }
        public void Hide()
        {
            //container.gameObject.SetActive(false);
            //isShowed = false;
        }
        public void OnClick(IPathBarHint hint)
        {
            pathBar.OnHintClicked(hint);
        }

        private void UpdateKeyboard()
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    selectedHint--;
                }
                else
                {
                    selectedHint++;
                }
            }
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
            var sortedHints = hints.Select(h => new KeyValuePair<IPathBarHint, int>(h, h.GetMatchesCount(field.text))).Where(kv => kv.Value > 0).OrderByDescending(kv => kv.Value);
            int hintsCount = Mathf.Min(pool.Length, sortedHints.Count());

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
            yield return new LocalFolderHint("C:/Windows", "Shindows");
            yield return new LocalFolderHint("C:/Users/REDIZIT/AppData/Roaming", "Roaming");

            var folders = Directory.EnumerateDirectories(startFolder);
            if (folders.Count() <= 100)
            {
                foreach (string folder in folders)
                {
                    yield return new SubFolderHint(startFolder, Path.GetFileName(folder));
                }
            }
        }
    }
}