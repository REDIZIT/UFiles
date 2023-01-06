using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace InApp
{
    public static class EntryUtils
    {
        public static EntryType GetType(string entryPath)
        {
            if (TryGetType(entryPath, out EntryType type)) return type;
            throw new System.Exception($"File not found at '{entryPath}'");
        }
        public static bool TryGetType(string entryPath, out EntryType entryType)
        {
            if (Directory.Exists(entryPath))
            {
                entryType = EntryType.Directory;
                return true;
            }
            if (File.Exists(entryPath))
            {
                entryType = EntryType.File;
                return true;
            }
            entryType = EntryType.Directory;
            return false;
        }

        public static void Move(string sourcePath, string targetPath)
        {
            EntryType type = GetType(sourcePath);

            if (type == EntryType.Directory) Directory.Move(sourcePath, targetPath);
            else File.Move(sourcePath, targetPath);
        }
        public static void Copy(string sourcePath, string targetPath, bool overwrite)
        {
            EntryType type = GetType(sourcePath);

            if (type == EntryType.Directory) CopyFilesRecursively(sourcePath, targetPath);
            else File.Copy(sourcePath, targetPath, overwrite);
        }
        public static void Delete(string entryPath)
        {
            if (TryGetType(entryPath, out EntryType type))
            {
                if (type == EntryType.Directory) Directory.Delete(entryPath, true);
                else File.Delete(entryPath);
            }
        }
        public static bool Exists(string entryPath)
        {
            return TryGetType(entryPath, out _);
        }
        public static string FindClosestFreeName(string entryPath)
        {
            EntryType type = GetType(entryPath);
            int index = 1;

            if (type == EntryType.Directory)
            {
                while (Directory.Exists(entryPath + " (" + index + ")") == false)
                {
                    index++;
                }
                return entryPath+ " (" + index + ")"; ;
            }
            else
            {
                var directory = new FileInfo(entryPath).Directory;
                string name = Path.GetFileNameWithoutExtension(entryPath);
                string extension = Path.GetExtension(entryPath);
                string entryNewPath = directory + "/" + name + " ({0})" + extension;
                while (File.Exists(string.Format(entryNewPath, index)))
                {
                    index++;
                }
                return string.Format(entryNewPath, index);
            }
        }

        private static void CopyFilesRecursively(string sourcePath, string targetPath)
        {
            Directory.CreateDirectory(targetPath);

            foreach (string dirPath in Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories))
            {
                Directory.CreateDirectory(dirPath.Replace(sourcePath, targetPath));
            }
            foreach (string newPath in Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories))
            {
                File.Copy(newPath, newPath.Replace(sourcePath, targetPath), true);
            }
        }
    }
}