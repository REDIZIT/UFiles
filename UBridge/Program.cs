using Etier.IconHelper;
using System.Drawing.Imaging;
using System.Drawing;

namespace UBridge
{
    public static class Program
    {
        public static void Main()
        {
            while (true)
            {
                ReceiveCommand();
            }
        }

        private static void ReceiveCommand()
        {
            GetFileIconCommand cmd = new GetFileIconCommand();
            cmd.Perform();
        }
    }
    public class GetFileIconCommand
    {
        public void Perform()
        {
            string path = Console.ReadLine();

            Console.WriteLine("Get icon for '" + path + "'");

            Icon icon = IconReader.GetFileIcon(path, IconReader.IconSize.Small, false);

            Bitmap map = icon.ToBitmap();

            map.Save(Console.OpenStandardOutput(), ImageFormat.Png);
        }
    }
}