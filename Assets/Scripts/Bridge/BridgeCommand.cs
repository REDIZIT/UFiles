using System.Diagnostics;
using Debug = UnityEngine.Debug;

namespace InApp
{
    public abstract class BridgeCommand
    {
        public Process process;

        public void Perform()
        {
            WriteLine(GetType().Name);

            OnPerform();
        }
        protected abstract void OnPerform();

        protected void WriteLine(string message)
        {
            process.StandardInput.WriteLine(message);
        }
        protected string ReadLine()
        {
            return process.StandardOutput.ReadLine();
        }
        protected void Log(string messsage)
        {
            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                Debug.Log(messsage);
            });
        }
    }
}