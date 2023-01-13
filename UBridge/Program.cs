using UFilesBridge.Scripts.Comm;

namespace UBridge
{
    public static class Program
    {
        public static void Main()
        {
            Console.WriteLine("UBridge 2");
            try
            {
                Communication.CreateConnection();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}