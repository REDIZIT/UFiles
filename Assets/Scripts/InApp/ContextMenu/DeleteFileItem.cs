using System;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;
using Zenject;

namespace InApp.UI
{
    public class DeleteFileItem : ContextItem
    {
        [Inject] private FilesView files;

        private bool isShifted;

        public DeleteFileItem()
        {
            text = "Удалить";
        }

        public override Texture2D GetIcon()
        {
            return icons.context.delete.texture;
        }
        public override void Update()
        {
            base.Update();
            isShifted = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
            text = isShifted ? "Удалить" : "В корзину";
        }

        public override void OnClick(ContextItemEnvironment env)
        {
            foreach (string filepath in env.selectedFiles)
            {
                if (isShifted)
                {
                    File.Delete(filepath);
                }
                else
                {
                    FileOperationAPIWrapper.Send(filepath);
                }
            }
        }


        public class FileOperationAPIWrapper
        {
            [Flags]
            public enum FileOperationFlags : ushort
            {
                /// <summary>
                /// Do not show a dialog during the process
                /// </summary>
                FOF_SILENT = 0x0004,
                /// <summary>
                /// Do not ask the user to confirm selection
                /// </summary>
                FOF_NOCONFIRMATION = 0x0010,
                /// <summary>
                /// Delete the file to the recycle bin.  (Required flag to send a file to the bin
                /// </summary>
                FOF_ALLOWUNDO = 0x0040,
                /// <summary>
                /// Do not show the names of the files or folders that are being recycled.
                /// </summary>
                FOF_SIMPLEPROGRESS = 0x0100,
                /// <summary>
                /// Surpress errors, if any occur during the process.
                /// </summary>
                FOF_NOERRORUI = 0x0400,
                /// <summary>
                /// Warn if files are too big to fit in the recycle bin and will need
                /// to be deleted completely.
                /// </summary>
                FOF_WANTNUKEWARNING = 0x4000,
            }

            public enum FileOperationType : uint
            {
                FO_MOVE = 0x0001,
                FO_COPY = 0x0002,
                FO_DELETE = 0x0003,
                FO_RENAME = 0x0004,
            }

            [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
            private struct SHFILEOPSTRUCT
            {

                public IntPtr hwnd;
                [MarshalAs(UnmanagedType.U4)]
                public FileOperationType wFunc;
                public string pFrom;
                public string pTo;
                public FileOperationFlags fFlags;
                [MarshalAs(UnmanagedType.Bool)]
                public bool fAnyOperationsAborted;
                public IntPtr hNameMappings;
                public string lpszProgressTitle;
            }

            [DllImport("shell32.dll", CharSet = CharSet.Auto)]
            private static extern int SHFileOperation(ref SHFILEOPSTRUCT FileOp);

            public static bool Send(string path, FileOperationFlags flags)
            {
                try
                {
                    var fs = new SHFILEOPSTRUCT
                    {
                        wFunc = FileOperationType.FO_DELETE,
                        pFrom = path + '\0' + '\0',
                        fFlags = FileOperationFlags.FOF_ALLOWUNDO | flags
                    };
                    SHFileOperation(ref fs);
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }

            public static bool Send(string path)
            {
                return Send(path, FileOperationFlags.FOF_NOCONFIRMATION | FileOperationFlags.FOF_WANTNUKEWARNING);
            }

            public static bool MoveToRecycleBin(string path)
            {
                return Send(path, FileOperationFlags.FOF_NOCONFIRMATION | FileOperationFlags.FOF_NOERRORUI | FileOperationFlags.FOF_SILENT);

            }

            private static bool DeleteFile(string path, FileOperationFlags flags)
            {
                try
                {
                    var fs = new SHFILEOPSTRUCT
                    {
                        wFunc = FileOperationType.FO_DELETE,
                        pFrom = path + '\0' + '\0',
                        fFlags = flags
                    };
                    SHFileOperation(ref fs);
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }

            public static bool DeleteCompletelySilent(string path)
            {
                return DeleteFile(path,
                                  FileOperationFlags.FOF_NOCONFIRMATION | FileOperationFlags.FOF_NOERRORUI |
                                  FileOperationFlags.FOF_SILENT);
            }
        }
    }
}