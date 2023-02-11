using System.Runtime.InteropServices;
using System;
using UnityEngine;

namespace InApp.Background
{
    public class BackgroundService
    {
        [DllImport("user32.dll")]
        private static extern IntPtr GetActiveWindow();

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        const int SW_HIDE = 0;
        const int SW_SHOW = 5;

        private IntPtr handle;

        public BackgroundService()
        {
            handle = GetActiveWindow();

            Application.wantsToQuit += OnWantToQuit;
        }
        ~BackgroundService()
        {
            Application.wantsToQuit -= OnWantToQuit;
        }

        private bool OnWantToQuit()
        {
            if (Application.isEditor || Input.GetKey(KeyCode.LeftShift))
            {
                return true;
            }

            HideApp();
            return false;
        }
        private void HideApp()
        {
            Application.targetFrameRate = 1;
            ShowWindow(handle, SW_HIDE);
        }
        private void ShowApp()
        {
            ShowWindow(handle, SW_SHOW);
        }
    }
}