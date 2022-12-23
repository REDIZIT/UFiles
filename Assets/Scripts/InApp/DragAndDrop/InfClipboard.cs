/********************************************************************++
Copyright (c) Microsoft Corporation.  All rights reserved.
--********************************************************************/

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using UnityEngine;
using static System.Windows.Forms.DataFormats;
using Debug = UnityEngine.Debug;

namespace Microsoft.PowerShell.Internal
{
    public static class Clipboard
    {
        public static string GetText()
        {
            //if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            //{
            //    PSConsoleReadLine.Ding();
            //    return "";
            //}

            string clipboardText = "";
            ExecuteOnStaThread(() => GetTextImpl(out clipboardText));

            return clipboardText;
        }

        public static void SetText(string text)
        {
            //if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            //{
            //    PSConsoleReadLine.Ding();
            //    return;
            //}

            ExecuteOnStaThread(() => SetClipboardData(Tuple.Create(text, CF_UNICODETEXT)));
        }
        public static void SetDataObject(object obj)
        {
            ExecuteOnStaThread(() =>
            {
                System.Windows.Forms.Clipboard.SetDataObject(obj);
                return true;
            });
        }
        public static void SetFIleDrop(string filename)
        {
            ExecuteOnStaThread(() => SetClipboardData(Tuple.Create(filename, CF_HDROP)));
        }

        public static void SetRtf(string plainText, string rtfText)
        {
            //if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            //{
            //    PSConsoleReadLine.Ding();
            //    return;
            //}

            if (CF_RTF == 0)
            {
                CF_RTF = RegisterClipboardFormat("Rich Text Format");
            }

            ExecuteOnStaThread(() => SetClipboardData(
                Tuple.Create(plainText, CF_UNICODETEXT),
                Tuple.Create(rtfText, CF_RTF)));
        }

        private const uint GMEM_MOVEABLE = 0x0002;
        private const uint GMEM_ZEROINIT = 0x0040;
        const uint GHND = GMEM_MOVEABLE | GMEM_ZEROINIT;

        [DllImport("kernel32.dll")]
        private static extern IntPtr GlobalAlloc(uint uFlags, UIntPtr dwBytes);

        [DllImport("kernel32.dll")]
        private static extern IntPtr GlobalFree(IntPtr hMem);

        [DllImport("kernel32.dll")]
        private static extern IntPtr GlobalLock(IntPtr hMem);

        [DllImport("kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GlobalUnlock(IntPtr hMem);

        [DllImport("kernel32.dll", EntryPoint = "CopyMemory", SetLastError = false)]
        private static extern void CopyMemory(IntPtr dest, IntPtr src, uint count);

        [DllImport("user32.dll", SetLastError = false)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool IsClipboardFormatAvailable(uint uFormat);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool OpenClipboard(IntPtr hWndNewOwner);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool CloseClipboard();

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool EmptyClipboard();

        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr GetClipboardData(uint uFormat);

        [DllImport("user32.dll")]
        private static extern IntPtr SetClipboardData(uint uFormat, IntPtr data);

        [DllImport("user32.dll", SetLastError = true)]
        static extern uint RegisterClipboardFormat(string lpszFormat);

        private const uint CF_TEXT = 1;
        private const uint CF_UNICODETEXT = 13;
        private const uint CF_HDROP = 15;
        private static uint CF_RTF;

        [StructLayout(LayoutKind.Sequential, Pack = 4 /* x86 */ )]
        struct DROPFILES
        {
            public int pFiles;
            public long pt;
            public int fNC;
            public int fWide;
        }

        private static bool GetTextImpl(out string text)
        {
            try
            {
                if (IsClipboardFormatAvailable(CF_UNICODETEXT))
                {
                    if (OpenClipboard(IntPtr.Zero))
                    {
                        var data = GetClipboardData(CF_UNICODETEXT);
                        if (data != IntPtr.Zero)
                        {
                            data = GlobalLock(data);
                            text = Marshal.PtrToStringUni(data);
                            GlobalUnlock(data);
                            return true;
                        }
                    }
                }
                else if (IsClipboardFormatAvailable(CF_TEXT))
                {
                    if (OpenClipboard(IntPtr.Zero))
                    {
                        var data = GetClipboardData(CF_TEXT);
                        if (data != IntPtr.Zero)
                        {
                            data = GlobalLock(data);
                            text = Marshal.PtrToStringAnsi(data);
                            GlobalUnlock(data);
                            return true;
                        }
                    }
                }
            }
            catch
            {
            }
            finally
            {
                CloseClipboard();
            }

            text = "";
            return false;
        }

        private static bool SetClipboardData(params Tuple<string, uint>[] data)
        {
            try
            {
                if (!OpenClipboard(IntPtr.Zero)) return false;
                EmptyClipboard();

                foreach (var d in data)
                {
                    if (!SetSingleClipboardData(d.Item1, d.Item2))
                        return false;
                }
            }
            finally
            {
                CloseClipboard();
            }

            return true;
        }

        private static bool SetSingleClipboardData(string text, uint format)
        {
            IntPtr hGlobal = IntPtr.Zero;
            IntPtr data = IntPtr.Zero;

            try
            {

                uint bytes;
                if (format == CF_RTF || format == CF_TEXT)
                {
                    bytes = (uint)(text.Length + 1);
                    data = Marshal.StringToHGlobalAnsi(text);
                }
                else if (format == CF_UNICODETEXT)
                {
                    bytes = (uint)((text.Length + 1) * 2);
                    data = Marshal.StringToHGlobalUni(text);
                }
                else
                {
                    // Not yet supported format.
                    return false;
                }

                if (data == IntPtr.Zero) return false;

                hGlobal = GlobalAlloc(GHND, (UIntPtr)bytes);
                if (hGlobal == IntPtr.Zero) return false;

                IntPtr dataCopy = GlobalLock(hGlobal);
                if (dataCopy == IntPtr.Zero) return false;
                CopyMemory(dataCopy, data, bytes);
                GlobalUnlock(hGlobal);

                if (SetClipboardData(format, hGlobal) != IntPtr.Zero)
                {
                    // The clipboard owns this memory now, so don't free it.
                    hGlobal = IntPtr.Zero;
                }
            }
            catch
            {
            }
            finally
            {
                if (data != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(data);
                }
                if (hGlobal != IntPtr.Zero)
                {
                    GlobalFree(hGlobal);
                }
            }

            return true;
        }

        private static void ExecuteOnStaThread(Func<bool> action)
        {
            const int retryCount = 5;
            int tries = 0;

            if (Thread.CurrentThread.GetApartmentState() == ApartmentState.STA)
            {
                while (tries++ < retryCount && !action())
                    ;
                return;
            }

            Exception exception = null;
            var thread = new Thread(() =>
            {
                try
                {
                    while (tries++ < retryCount && !action())
                        ;
                }
                catch (Exception e)
                {
                    exception = e;
                }
            });

            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            thread.Join();

            if (exception != null)
            {
                throw exception;
            }
        }

        const uint WM_DROPFILES = 0x233;

        public static unsafe void DropFiles(string[] files)
        {
            var ptr = GetDropFilesPtr(files);
            var handle = Process.GetProcessesByName("explorer")[0].Handle;
            var ret = PostMessage((IntPtr)handle /* Main MPC window */, WM_DROPFILES, ptr, IntPtr.Zero);
            System.Threading.Thread.Sleep(100);
            GlobalFree(ptr);
        }
        public static unsafe IntPtr GetDropFilesPtr(string[] files)
        {
            int memAllocSize = 0; // in bytes
            var dropFiles = new DROPFILES();
            var pMem = IntPtr.Zero;
            var pLockedMem = IntPtr.Zero;
            var ptr = (byte*)0;

            dropFiles.fWide = 1;
            dropFiles.pFiles = Marshal.SizeOf(dropFiles);

            for (int i = 0; i < files.Length; ++i)
                memAllocSize += (files[i].Length * 2) + 2 /* \0\0 */;
            memAllocSize += 2; // \0\0
            memAllocSize += dropFiles.pFiles;

            pMem = GlobalAlloc(GHND, (UIntPtr)memAllocSize);
            pLockedMem = GlobalLock(pMem);
            {
                // copy files names to unmanaged mem
                *(DROPFILES*)(void*)pLockedMem = dropFiles;
                ptr = (byte*)pLockedMem + dropFiles.pFiles;

                for (int i = 0; i < files.Length; ++i)
                {
                    int count = files[i].Length * 2;
                    fixed (char* pBuff = files[i])
                        memcpy(ptr, pBuff, count);
                    ptr += count + 2;
                }
            }

            if (!GlobalUnlock(pMem) && Marshal.GetLastWin32Error() != 0)
            {
                GlobalFree(pMem);
                pMem = IntPtr.Zero;
                Debug.Log("Free");
            }

            if (SetClipboardData(CF_HDROP, pMem) != IntPtr.Zero)
            {
                Debug.Log("Dont free");
                // The clipboard owns this memory now, so don't free it.
                pMem = IntPtr.Zero;
            }

            return pMem;
        }
        [DllImport("ntdll.dll", CallingConvention = CallingConvention.Cdecl)]
        unsafe static extern void* memcpy(
            [In] void* destination,
            [In] void* source,
            [In] long num // for x86
                         // [In] long num // for x64
            );
        [DllImport( "user32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        static extern int PostMessage(
            [In] IntPtr hWnd,
            [In] uint Msg,
            [In] IntPtr wParam,
            [In] IntPtr lParam
            );
    }
}