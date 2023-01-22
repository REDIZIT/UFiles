using Etier.IconHelper;
using System.Drawing.Imaging;
using System.Drawing;
using System.Reflection;

namespace UBridge
{
    public static class Program
    {
        private static Dictionary<string, Type> typeByName = new Dictionary<string, Type>();

        public static void Main()
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