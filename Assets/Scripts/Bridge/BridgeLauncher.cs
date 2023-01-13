using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Pipes;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace InApp
{
    public class BridgeLauncher : MonoBehaviour
    {
        [SerializeField] private RawImage image;

        private TcpClient client;

        void Start ()
        {
            DateTime t = DateTime.Now;
            StartBridge();
            StartTCP();
            UnityEngine.Debug.Log("Taken: " + (DateTime.Now - t).TotalMilliseconds + "ms");
        }
        private void OnApplicationQuit()
        {
            client.Close();
        }

        private void StartBridge()
        {
            string exePath = Application.streamingAssetsPath + "/Release/net7.0/UFilesBridge.exe";
            Process.Start(exePath);
        }
        private void StartTCP()
        {
            client = new TcpClient("localhost", 209);

            UnityEngine.Debug.Log("Connected");

            var stream = client.GetStream();
            string path = "C:/Test/1.txt";
            stream.Write(Encoding.UTF8.GetBytes(path), 0, path.Length);

            byte[] buffer = new byte[1024 * 4];
            int bytesCount = stream.Read(buffer, 0, buffer.Length);

            //byte[] bytes = ReceiveAll(client.Client);

            UnityEngine.Debug.Log("Bytes count = " + bytesCount);

            Texture2D tex = new Texture2D(1, 1);
            tex.LoadImage(buffer);

            image.texture = tex;
        }
        private byte[] ReceiveAll(Socket socket)
        {
            var buffer = new List<byte>();
            while (socket.Available > 0)
            {
                var currByte = new Byte[1];
                var byteCounter = socket.Receive(currByte, currByte.Length, SocketFlags.None);
                if (byteCounter.Equals(1))
                {
                    buffer.Add(currByte[0]);
                }
            }
            return buffer.ToArray();
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