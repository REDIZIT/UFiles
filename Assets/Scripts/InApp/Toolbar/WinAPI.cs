using System.Runtime.InteropServices;

namespace InApp.Toolbar
{
    public static class WinAPI
    {
        [DllImport("user32.dll")]
        public static extern System.IntPtr GetActiveWindow();

        [DllImport("user32.dll")]
        public static extern int FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll")]
        public  static extern bool SetWindowPos(
            System.IntPtr hWnd, // window handle
            System.IntPtr hWndInsertAfter, // placement order of the window
            short X, // x position
            short Y, // y position
            short cx, // width
            short cy, // height
            uint uFlags // window flags.
        );

        [DllImport("user32.dll")]
        public static extern System.IntPtr SetWindowLong(
             System.IntPtr hWnd, // window handle
             int nIndex,
             uint dwNewLong
        );

        [DllImport("user32.dll")]
        public static extern System.IntPtr GetWindowLong(
            System.IntPtr hWnd,
            int nIndex
        );

        [DllImport("user32.dll")]
        public static extern void ShowWindow(
            System.IntPtr hWnd,
            int nCmdShow
        );
    }
}