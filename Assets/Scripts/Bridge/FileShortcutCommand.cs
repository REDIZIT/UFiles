namespace InApp
{
    public class FileShortcutCommand : BridgeCommand
    {
        private string linkPathWithoutExtension, appFilePath;

        public FileShortcutCommand(string linkPathWithoutExtension, string appFilePath)
        {
            this.linkPathWithoutExtension = linkPathWithoutExtension;
            this.appFilePath = appFilePath;
        }

        protected override void OnPerform()
        {
            WriteLine(appFilePath);
            WriteLine(linkPathWithoutExtension);
        }
    }
}