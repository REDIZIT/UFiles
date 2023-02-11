namespace UBridge.Scripts.Commands.Archive
{
    public class ExtractArchiveCommand : ICommand
    {
        public void Perform()
        {
            string archivePath = Console.ReadLine();
            string tempFolderPath = Console.ReadLine();

            try
            {
                if (Directory.Exists(tempFolderPath))
                {
                    foreach (var item in Directory.EnumerateFiles(tempFolderPath))
                    {
                        File.Delete(item);
                    }
                    foreach (var item in Directory.EnumerateDirectories(tempFolderPath))
                    {
                        Directory.Delete(item);
                    }
                }
                else
                {
                    Directory.CreateDirectory(tempFolderPath);
                }

                DateTime t = DateTime.Now;

                SharpCompress.Archives.ArchiveFactory.WriteToDirectory(archivePath, tempFolderPath, new SharpCompress.Common.ExtractionOptions()
                {
                    ExtractFullPath = true
                });

                Console.WriteLine("Extracted in " + (DateTime.Now - t).TotalMilliseconds + "ms");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }



        }
    }
}
