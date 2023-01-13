using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Pipes;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using Debug = UnityEngine.Debug;

namespace InApp
{
    public class BridgeLauncher : MonoBehaviour
    {
        [SerializeField] private RawImage image;

        private TcpClient client;
        private Thread t;

        void Start ()
        {
            t = new Thread(StartBridge);
            t.Start();
        }
        private void OnApplicationQuit()
        {
            client.Close();
            t.Abort();
        }

        private void StartBridge()
        {
            Stopwatch w = Stopwatch.StartNew();

            StartProcess();

            w.Stop();
            Log("Start time: " + w.ElapsedMilliseconds);
            w.Restart();

            StartTCP();

            Log("TCP time: " + w.ElapsedMilliseconds);
        }
        private void StartProcess()
        {
            string exePath = Application.streamingAssetsPath + "/Release/net7.0/UFilesBridge.exe";
            Process.Start(exePath);
        }
        private void StartTCP()
        {
            Stopwatch w = Stopwatch.StartNew();

            client = new TcpClient();

            Log("Connected");

            client.Connect(new IPAddress(new byte[] { 127, 0, 0, 1 }), 1703);

            Log("Connected in " + w.ElapsedMilliseconds);
            w.Restart();

            var stream = client.GetStream();
            string path = "C:/Test/1.txt";
            stream.Write(Encoding.UTF8.GetBytes(path), 0, path.Length);

            Log("Sent command in " + w.ElapsedMilliseconds);
            w.Restart();

            byte[] buffer = new byte[1024 * 4];
            stream.Read(buffer, 0, buffer.Length);

            Log("Received command in " + w.ElapsedMilliseconds);
            w.Restart();


            

            Log("Texture2D created in " + w.ElapsedMilliseconds);
            w.Restart();

            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                Texture2D tex = new Texture2D(1, 1);
                tex.LoadImage(buffer);

                image.texture = tex;
            });

            Log("Image set in " + w.ElapsedMilliseconds);
        }
        private void Log(string messsage)
        {
            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                Debug.Log(messsage);
            });
        }
    }
    public class TestCommand
    {
        public void Perform(NetworkStream socket)
        {
            Write(socket, "TestCommand;123");
        }

        public void Write(NetworkStream socket, string str)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(str);
            socket.Write(bytes, 0, bytes.Length);
        }
    }
}