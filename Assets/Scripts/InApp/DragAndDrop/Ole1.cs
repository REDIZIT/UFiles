using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

class Program
{
    [DllImport("kernel32.dll")]
    static extern IntPtr GlobalLock(IntPtr hMem);
    [DllImport("kernel32.dll")]
    static extern IntPtr GlobalUnlock(IntPtr hMem);
    [DllImport("user32.dll", SetLastError = true)]
    static extern bool OpenClipboard(IntPtr hWndNewOwner);
    [DllImport("user32.dll")]
    static extern bool EmptyClipboard();
    [DllImport("user32.dll")]
    static extern IntPtr SetClipboardData(uint uFormat, IntPtr hData);
    [DllImport("user32.dll", SetLastError = true)]
    static extern bool CloseClipboard();

    [StructLayout(LayoutKind.Sequential)]
    struct DROPFILES
    {
        public uint pFiles;
        public int x;
        public int y;
        public int fNC;
        public int fWide;
    };

    public static byte[] StructureToByte<T>(T structure)
    {
        int size = Marshal.SizeOf(typeof(T));
        byte[] buffer = new byte[size];
        IntPtr bufferIntPtr = Marshal.AllocHGlobal(size);
        try
        {
            Marshal.StructureToPtr(structure, bufferIntPtr, true);
            Marshal.Copy(bufferIntPtr, buffer, 0, size);
        }
        finally
        {
            Marshal.FreeHGlobal(bufferIntPtr);
        }
        return buffer;
    }

    public static void SetClipboardData(List<string> pathList)
    {
        StringBuilder builder = new StringBuilder();
        for (int i = 0; i < pathList.Count; i++)
        {
            builder.Append(pathList[i]);
            builder.Append('\0');
        }
        builder.Append('\0');
        string path = builder.ToString();
        OpenClipboard(IntPtr.Zero);
        int length = Marshal.SizeOf(typeof(DROPFILES));
        IntPtr bufferPtr = Marshal.AllocHGlobal(length + path.Length * sizeof(char) + 8);
        try
        {
            GlobalLock(bufferPtr);
            DROPFILES config = new DROPFILES();
            config.pFiles = (uint)length;
            config.fNC = 1;
            int seek = 0;
            byte[] configData = StructureToByte(config);
            for (int i = 0; i < configData.Length; i++)
            {
                Marshal.WriteByte(bufferPtr, seek, configData[i]);
                seek++;
            }
            for (int i = 0; i < path.Length; i++)
            {
                Marshal.WriteInt16(bufferPtr, seek, path[i]);
                seek++;
            }
            GlobalUnlock(bufferPtr);
            EmptyClipboard();
            SetClipboardData(15, bufferPtr);
        }
        catch (Exception e)
        {
            throw e;
        }
        finally
        {
            Marshal.FreeHGlobal(bufferPtr);
            CloseClipboard();
        }
    }
    static void Main(string[] args)
    {
        SetClipboardData(new List<string> { "D:\\666.txt", "D:\\333.txt" });
        Console.Read();
    }
}
