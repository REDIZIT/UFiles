using DOExample;
using System;
using System.IO;
using System.Threading;
using System.Windows;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

namespace InApp
{
    public static class DragAndDrop
    {
        public static void Start(string file)
        {
            Debug.Log("Start");
            //Int32 NumItems = 1;

            //DataObjectEx.SelectedItem[] SelectedItems = new DataObjectEx.SelectedItem[NumItems];
            //for (Int32 ItemCount = 0; ItemCount < SelectedItems.Length; ItemCount++)
            //{
            //    // TODO: Get virtual file name
            //    SelectedItems[ItemCount].FileName = "Some name";
            //    SelectedItems[ItemCount].SourceFileName = "C:/1.txt";
            //    // TODO: Get virtual file date
            //    SelectedItems[ItemCount].WriteTime = new DateTime(2022, 12, 23);
            //    // TODO: Get virtual file size
            //    SelectedItems[ItemCount].FileSize = new FileInfo(file).Length;
            //}
            //DataObjectEx dataObject = new DataObjectEx(SelectedItems);
            //dataObject.SetData(NativeMethods.CFSTR_FILEDESCRIPTORW, null);
            //dataObject.SetData(NativeMethods.CFSTR_FILECONTENTS, null);
            //dataObject.SetData(NativeMethods.CFSTR_PERFORMEDDROPEFFECT, null);

            Debug.Log(GUIUtility.systemCopyBuffer);

            //Clipboard.SetText("Clipped text", TextDataFormat.UnicodeText);
            //Clipboard.SetDataObject(dataObject);

            Debug.Log("Copied");
        }
    }
}