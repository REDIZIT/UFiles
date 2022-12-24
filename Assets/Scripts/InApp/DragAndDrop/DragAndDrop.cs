using System;
using System.Windows.Forms;
using UnityEngine;

namespace InApp
{
    public static class DragAndDrop
    {
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

            Program.SetClipboardData(new System.Collections.Generic.List<string>() { "C:/1.txt" });

            

            Debug.Log("Copied");
        }

        private static void Control_QueryContinueDrag(object sender, QueryContinueDragEventArgs e)
        {
            Debug.Log("Control_QueryContinueDrag");
        }
    }
}