﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.Profiling;
using Debug = UnityEngine.Debug;

namespace InApp
{
    public class Bridge
    {
        private Dictionary<string, Action<Texture2D>> requests = new Dictionary<string, Action<Texture2D>>();
        private Dictionary<string, byte[]> loadedIcons = new Dictionary<string, byte[]>();
        private List<string> notHandledPathes = new List<string>();

        private Process process;
        private Thread t;

        public void Start()
        {
            t = new Thread(StartBridge);
            t.Start();
        }
        public void Stop()
        {
            if (process != null && process.HasExited == false)
            {
                process.Kill();
            }
            t?.Abort();
        }
        public void UnityUpdate()
        {
            Profiler.BeginSample("Bridge texture creation");
            foreach (var key in loadedIcons.Keys.ToArray())
            {
                Texture2D tex = new Texture2D(1, 1);
                tex.LoadImage(loadedIcons[key]);

                requests[key]?.Invoke(tex);
                requests.Remove(key);

                Debug.Log("Texture created for " + key);
            }

            loadedIcons.Clear();
            Profiler.EndSample();
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
                requests.Add(filepath, callback);
                notHandledPathes.Add(filepath);
            }
            
        }

        private void StartBridge()
        {
            StartProcess();
            StartSendLoop();
        }
        private void StartProcess()
        {
            string exePath = Application.streamingAssetsPath + "/Release/net7.0/UBridge.exe";
            ProcessStartInfo info = new ProcessStartInfo(exePath);
            info.CreateNoWindow = true;

            info.UseShellExecute = false;
            info.RedirectStandardInput = true;
            info.RedirectStandardOutput = true;

            process = Process.Start(info);
        }
        private void StartSendLoop()
        {
            while (true)
            {
                if (notHandledPathes.Count > 0)
                {
                    SendCommand(notHandledPathes[0]);
                    notHandledPathes.RemoveAt(0);
                }
            }
        }
        private void SendCommand(string path)
        {
            Log("Send " + path);
            var stream = process.StandardInput;
            stream.WriteLine(path);

            var outstream = process.StandardOutput.BaseStream;

            Log(process.StandardOutput.ReadLine());

            byte[] buffer = new byte[1024];
            outstream.Read(buffer, 0, buffer.Length);
            loadedIcons.Add(path, buffer);
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