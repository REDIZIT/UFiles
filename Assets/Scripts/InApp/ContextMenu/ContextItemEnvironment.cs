using System.Collections.Generic;
using System.Linq;

namespace InApp.UI
{
    public class ContextItemEnvironment
    {
        public string currentFolder;
        public string selectedEntryName => selectedFiles.LastOrDefault();
        public IEnumerable<string> selectedFiles;
    }
}