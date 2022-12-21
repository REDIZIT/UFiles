using UnityEngine;

namespace InApp.Toolbar
{
    public class WindowToolbarButton : MonoBehaviour
    {
        public void CloseApp()
        {
            Application.Quit();
        }
        public void MinimizeApp()
        {
            WinAPI.ShowWindow(WinAPI.GetActiveWindow(), 6);
        }
        public void MaximizeApp()
        {
            WinAPI.ShowWindow(WinAPI.GetActiveWindow(), 3);
        }
    }
}