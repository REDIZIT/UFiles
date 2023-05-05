using IWshRuntimeLibrary;
using File = System.IO.File;

namespace UBridge.Scripts.Commands
{
    public class FileShortcutCommand : ICommand
    {
        public void Perform()
        {
            string appFilePath = Console.ReadLine();
            string linkPathWithoutExtension = Console.ReadLine();

            if (File.Exists(linkPathWithoutExtension + ".lnk"))
            {
                File.Delete(linkPathWithoutExtension + ".lnk");
            }
            else
            {
                CreateShortcut(linkPathWithoutExtension, appFilePath);
            }
        }

        private void CreateShortcut(string linkPathWithoutExtension, string appFilePath)
        {
            WshShell shell = new WshShell();
            string shortcutAddress = linkPathWithoutExtension + ".lnk";
            IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(shortcutAddress);
            //shortcut.Description = "New shortcut for a Notepad";
            //shortcut.Hotkey = "Ctrl+Shift+N";
            shortcut.TargetPath = appFilePath;
            shortcut.Save();
        }
    }
}
