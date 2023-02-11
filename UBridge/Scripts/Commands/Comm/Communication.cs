using Etier.IconHelper;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Security.Principal;
using System.Text;

namespace UBridge.Scripts.Commands.Comm
{
    internal static class Communication
    {
        private static TcpClient client;
        private static int index;

        public static void CreateConnection()
        {
            CreateTCP();
        }
        private static void CreateTCP()
        {
            TcpListener listner = new TcpListener(IPAddress.Any, 1703);

            Console.WriteLine("Start listening");

            Stopwatch w = Stopwatch.StartNew();

            listner.Start();

            Console.WriteLine("Listen started in " + w.ElapsedMilliseconds);
            w.Restart();

            client = listner.AcceptTcpClient();
            client.ReceiveTimeout = 15;

            Console.WriteLine("Accepted in " + w.ElapsedMilliseconds);
            w.Stop();


            while (IsSocketConnected(client.Client))
            {
                if (client.Available > 0)
                {
                    ReceiveCommand();
                }
                else
                {
                    Thread.Sleep(10);
                }
            }

            Console.WriteLine("Disconnected");
        }
        private static void ReceiveCommand()
        {
            var stream = client.GetStream();

            byte[] buffer = new byte[1024];
            stream.Read(buffer);
            string path = Encoding.UTF8.GetString(buffer).TrimEnd((char)0);

            Console.WriteLine("Get icon for '" + path + "'");

            stream.Flush();

            //Icon icon = Icon.ExtractAssociatedIcon(path);
            Icon icon = IconReader.GetFileIcon(path, IconReader.IconSize.Small, false);

            Bitmap map = icon.ToBitmap();

            map.Save(stream, ImageFormat.Png);

            stream.Flush();
        }
        private static bool IsSocketConnected(Socket s)
        {
            bool part1 = s.Poll(1000, SelectMode.SelectRead);
            bool part2 = s.Available == 0;
            if (part1 && part2)
                return false;
            else
                return true;
        }
    }
}
