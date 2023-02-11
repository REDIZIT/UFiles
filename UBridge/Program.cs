using Etier.IconHelper;
using System.Drawing.Imaging;
using System.Drawing;
using System.Reflection;
using System.Text;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace UBridge
{
    public static class Program
    {
        private static Dictionary<string, Type> typeByName = new Dictionary<string, Type>();

        [DllImport("user32.dll")]
        private static extern IntPtr GetActiveWindow();

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        const int SW_HIDE = 0;
        const int SW_SHOW = 5;

        public static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;

            if (args.Length >= 1)
            {
                string cmd = args[0];
                if (cmd == "run-or-show")
                {
                    Process unityProcess = Process.GetProcessesByName("UFiles").FirstOrDefault();
                    if (unityProcess == null)
                    {
                        unityProcess = Process.Start("C:\\Users\\REDIZIT\\Documents\\GitHub\\UFiles\\Build\\UFiles.exe");
                    }
                    else
                    {
                        IntPtr handle = unityProcess.MainWindowHandle;
                        ShowWindow(handle, SW_SHOW);
                    }
                }
            }
            else
            {
                StartUpCommandHandling();
            }
        }

        private static void FixTaskbar()
        {

        }
        private static void StartUpCommandHandling()
        {
            BakeCommandTypes();

            while (true)
            {
                ReceiveCommand();
            }
        }

        private static void BakeCommandTypes()
        {
            typeByName.Clear();

            foreach (var type in Assembly.GetExecutingAssembly().GetTypes().Where(t => t.IsInterface == false && typeof(ICommand).IsAssignableFrom(t)).ToArray())
            {
                typeByName.Add(type.Name, type);
            }
        }
        private static void ReceiveCommand()
        {
            string commandName = Console.ReadLine();
            
            Type type = typeByName[commandName];

            ICommand cmd = Activator.CreateInstance(type) as ICommand;
            cmd.Perform();
        }
    }
    public interface ICommand
    {
        void Perform();
    }
    public class GetFileIconCommand : ICommand
    {
        public void Perform()
        {
            string path = Console.ReadLine();

            Icon icon = IconReader.GetFileIcon(path, IconReader.IconSize.Small, false);

            Bitmap map = icon.ToBitmap();

            map.Save(Console.OpenStandardOutput(), ImageFormat.Png);
        }
    }
}