using B83.Win32;
using InApp.UI;
using System.Collections.Generic;
using UnityEngine;

namespace InApp
{
    public static class DragAndDrop
    {
        private static FileOperator fileOperator;
        private static FilesView files;

        public static void RegisterWindowTarget(FileOperator fileOperator, FilesView files)
        {
            DragAndDrop.fileOperator = fileOperator;
            DragAndDrop.files = files;

            UnityDragAndDropHook.InstallHook();
            UnityDragAndDropHook.OnDroppedFiles += OnDropFromDesktop;
        }
        public static void UnregisterWindowTarget()
        {
            UnityDragAndDropHook.UninstallHook();
        }
        public static void Start(string file)
        {
            Debug.Log("Start");

            //Clipboard.ContainsText();
            //Clipboard.Clear();

            //Debug.Log(Microsoft.PowerShell.Internal.Clipboard.GetText());
            //Microsoft.PowerShell.Internal.Clipboard.SetText("12345");
            //Microsoft.PowerShell.Internal.Clipboard.GetDropFilesPtr(new string[] { "C:/1.txt"});

            //DataObject data = new();
            //data.SetText("Example");

            //var control = new Control();
            //control.QueryContinueDrag += Control_QueryContinueDrag;
            //control.DoDragDrop(data, DragDropEffects.All);

            Program.SetClipboardData(new List<string>() { "C:/1.txt" });

            Debug.Log("Copied");
        }

        private static void OnDropFromDesktop(List<string> aFiles, POINT aPos)
        {
            fileOperator.Run(new FileCopyOperation(aFiles, files.CurrentPath));
        }
    }
}