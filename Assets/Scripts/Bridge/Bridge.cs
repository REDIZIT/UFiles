﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using UnityEngine;

namespace InApp
{
    public class Bridge
    {
        private Queue<BridgeCommand> commands = new Queue<BridgeCommand>();

        private Process process;
        private Thread t;

        public void Start()
        {
            t = new Thread(ThreadedBridgeLoop);
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
        public void Enqueue(BridgeCommand command)
        {
            commands.Enqueue(command);
        }
        private void ThreadedBridgeLoop()
        {
            StartProcess();
            EnterCommandLoop();
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
        private void EnterCommandLoop()
        {
            while (true)
            {
                if (commands.Count > 0)
                {
                    var cmd = commands.Dequeue();
                    cmd.process = process;
                    cmd.Perform();
                }
                Thread.Sleep(10);
            }
        }        
    }
}