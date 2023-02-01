using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using TMPro;
using UnityEngine;
using Zenject;

namespace InApp
{
    public class SidebarProjectUI : MonoBehaviour
    {
        [SerializeField] private Project project;

        [SerializeField] private TextMeshProUGUI nameText, pathText;
        [SerializeField] private Transform urlsContent;

        private UrlButton.Pool urlPool;

        [DllImport("shlwapi.dll", CharSet = CharSet.Auto)]
        static extern bool PathCompactPathEx([Out] StringBuilder pszOut, string szPath, int cchMax, int dwFlags);

        private static string PathShortener(string path, int length)
        {
            StringBuilder sb = new StringBuilder(length + 1);
            PathCompactPathEx(sb, path, length, 0);
            return sb.ToString();
        }

        [Inject]
        private void Construct(UrlButton.Pool urlPool)
        {
            this.urlPool = urlPool;

            var mainFolderInfo = new DirectoryInfo(project.mainFolder);
            nameText.text = mainFolderInfo.Name;
            pathText.text = PathShortener(mainFolderInfo.Parent.FullName, 28);

            foreach (ProjectLink link in project.links)
            {
                var inst = urlPool.Spawn(link);
                inst.transform.SetParent(urlsContent);
            }
        }

        public void OnRunClicked()
        {
            Process.Start(project.buildPath);
        }
    }
}