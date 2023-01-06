using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace InApp.UI
{
    public static class EntriesSorter
    {
        public static IEnumerable<string> Sort(IEnumerable<string> entries, FolderSortingData.Type sortingType)
        {
            if (sortingType == FolderSortingData.Type.None) return ByNone(entries);
            if (sortingType == FolderSortingData.Type.ByName) return ByName(entries);
            if (sortingType == FolderSortingData.Type.ByDate) return ByDate(entries);
            if (sortingType == FolderSortingData.Type.BySize) return BySize(entries);

            throw new NotImplementedException();
        }
        private static IEnumerable<string> ByNone(IEnumerable<string> entries)
        {
            return entries;
        }
        private static IEnumerable<string> ByName(IEnumerable<string> entries)
        {
            return entries.OrderBy(e =>
            {
                int index = e.Replace(@"\", "/").LastIndexOf('/');
                string name = e.Substring(index + 1, e.Length - index - 1);
                return name;
            });
        }
        private static IEnumerable<string> ByDate(IEnumerable<string> entries)
        {
            return entries.OrderByDescending(e =>
            {
                if (Directory.Exists(e))
                {
                    return new DirectoryInfo(e).LastWriteTime;
                }
                else
                {
                    return new FileInfo(e).LastWriteTime;
                }
            });
        }
        private static IEnumerable<string> BySize(IEnumerable<string> entries)
        {
            return entries.OrderByDescending(e =>
            {
                if (Directory.Exists(e))
                {
                    // Move folders on top
                    return long.MaxValue;
                }
                else
                {
                    return new FileInfo(e).Length;
                }
            });
        }

    }

}