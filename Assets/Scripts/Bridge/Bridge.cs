using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace InApp
{
    public class Bridge
    {
        private Dictionary<string, Action<Texture2D>> requests = new Dictionary<string, Action<Texture2D>>();
        private Dictionary<string, byte[]> loadedIcons = new Dictionary<string, byte[]>();
        private List<string> notHandledPathes = new List<string>();

        private TcpClient client;
        private Thread t;

        public void Start()
        {
            t = new Thread(StartBridge);
            t.Start();
        }
        public void UnityUpdate()
        {
            foreach (var kv in loadedIcons)
            {
                Texture2D tex = new Texture2D(1, 1);
                tex.LoadImage(kv.Value);

                requests[kv.Key]?.Invoke(tex);
                requests.Remove(kv.Key);

                Debug.Log("Texture created for " + kv.Key);
            }

            loadedIcons.Clear();
        }
        public void RequestIcon(string filepath, Action<Texture2D> callback)
        {
            filepath = Path.GetExtension(filepath);

            if (requests.ContainsKey(filepath))
            {
                requests[filepath] += callback;
            }
            else
            {
                Debug.Log("Request " + filepath);
                requests.Add(filepath, callback);
                notHandledPathes.Add(filepath);
            }
            
        }

        private void StartBridge()
        {
            Stopwatch w = Stopwatch.StartNew();

            StartProcess();

            w.Stop();
            Log("Start time: " + w.ElapsedMilliseconds);
            w.Restart();

            StartTCP();

            Log("Bridge work time: " + w.ElapsedMilliseconds);
        }
        private void StartProcess()
        {
            string exePath = Application.streamingAssetsPath + "/Release/net7.0/UFilesBridge.exe";
            Process.Start(exePath);
        }
        private void StartTCP()
        {
            client = new TcpClient();
            client.Connect(new IPAddress(new byte[] { 127, 0, 0, 1 }), 1703);

            while (true)
            {
                if (notHandledPathes.Count > 0)
                {
                    SendCommand(notHandledPathes[0]);
                    notHandledPathes.RemoveAt(0);
                }
                Thread.Sleep(100);
            }
        }
        private void SendCommand(string path)
        {
            var stream = client.GetStream();
            Debug.Log("Send request " + path);
            stream.Write(Encoding.UTF8.GetBytes(path), 0, path.Length);

            byte[] buffer = new byte[1024 * 2];
            stream.Read(buffer, 0, buffer.Length);

            loadedIcons.Add(path, buffer);

            stream.Flush();
        }
        private void Log(string messsage)
        {
            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                Debug.Log(messsage);
            });
        }
    }
}