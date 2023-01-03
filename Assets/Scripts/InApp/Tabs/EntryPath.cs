using System.Runtime.InteropServices;
using System;
using System.IO;

namespace InApp
{
    public class EntryPath : IPath
    {
        [DllImport("shell32.dll", CharSet = CharSet.Unicode)]
        private static extern uint SHParseDisplayName(string pszName, IntPtr zero, [Out] out IntPtr ppidl, uint sfgaoIn, [Out] out uint psfgaoOut);

        [DllImport("shell32.dll", CharSet = CharSet.Unicode)]
        private static extern uint SHGetNameFromIDList(IntPtr pidl, SIGDN sigdnName, [Out] out String ppszName);

        private enum SIGDN : uint
        {
            PARENTRELATIVEEDITING = 0x80031001
        }


        private string path;

        public EntryPath(string path)
        {
            this.path = path;
        }
        public void Set(string path)
        {
            this.path = path;
        }
        public string GetDisplayName()
        {
            if (File.Exists(path))
            {
                // Entry is file
                return Path.GetFileName(path);
            }
            else
            {
                // Entry is folder or drive
                DirectoryInfo d = new DirectoryInfo(path);
                if (d.Parent == null)
                {
                    // Entry is drive
                    var info = new DriveInfo(path);
                    return GetDriveLabel(path) + " (" + info.VolumeLabel + ")";
                }
                else
                {
                    // Entry is regular folder
                    var info = new DirectoryInfo(path);
                    return info.Name;
                }
            }
        }
        public string GetFullPath()
        {
            return path;
        }

        private string GetDriveLabel(string driveNameAsLetterColonBackslash)
        {
            if (SHParseDisplayName(driveNameAsLetterColonBackslash.Replace("/", @"\"), IntPtr.Zero, out IntPtr pidl, 0, out _) == 0
                && SHGetNameFromIDList(pidl, SIGDN.PARENTRELATIVEEDITING, out string name) == 0
                && name != null)
            {
                return name;
            }
            return null;
        }
    }
}