using InApp.Toolbar;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class Borderless : MonoBehaviour
{
    #region DLLstuff

    const int SWP_HIDEWINDOW = 0x80; //hide window flag.
    const int SWP_SHOWWINDOW = 0x40; //show window flag.
    const int SWP_NOMOVE = 0x0002; //don't move the window flag.
    const int SWP_NOSIZE = 0x0001; //don't resize the window flag.
    const uint WS_SIZEBOX = 0x00040000;
    const int GWL_STYLE = -16;
    const int WS_BORDER = 0x00800000; //window with border
    const int WS_DLGFRAME = 0x00400000; //window with double border but no title
    const int WS_CAPTION = WS_BORDER | WS_DLGFRAME; //window with a title bar
    const int WS_SYSMENU = 0x00080000;      //window with no borders etc.
    const int WS_MAXIMIZEBOX = 0x00010000;
    const int WS_MINIMIZEBOX = 0x00020000;  //window with minimizebox

    System.IntPtr hWnd;
    System.IntPtr HWND_TOP = new System.IntPtr(0);
    System.IntPtr HWND_TOPMOST = new System.IntPtr(-1);
    System.IntPtr HWND_NOTOPMOST = new System.IntPtr(-2);

    #endregion

    [SerializeField] bool hideOnStart = false;

    public void ShowWindowBorders(bool value)
    {
        if (Application.isEditor) return; //We don't want to hide the toolbar from our editor!

        int style = WinAPI.GetWindowLong(hWnd, GWL_STYLE).ToInt32(); //gets current style

        if (value)
        {
            WinAPI.SetWindowLong(hWnd, GWL_STYLE, (uint)(style | WS_CAPTION | WS_SIZEBOX)); //Adds caption and the sizebox back.
            WinAPI.SetWindowPos(hWnd, HWND_NOTOPMOST, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_SHOWWINDOW); //Make the window normal.
        }
        else
        {
            WinAPI.SetWindowLong(hWnd, GWL_STYLE, (uint)(style & ~(WS_CAPTION | WS_SIZEBOX))); //removes caption and the sizebox from current style.
            WinAPI.SetWindowPos(hWnd, HWND_NOTOPMOST, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_SHOWWINDOW); //Make the window render above toolbar.
        }
    }

    private void Awake()
    {
        hWnd = WinAPI.GetActiveWindow();
    }
    private void Start()
    {
        if (hideOnStart) ShowWindowBorders(false);
    }

}