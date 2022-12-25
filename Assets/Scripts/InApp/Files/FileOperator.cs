using InApp.UI;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace InApp
{
    public class FileOperator
    {
        public Action onAnyOperationApplied;

        private History<FileOperation> history = new History<FileOperation>(HistoryPointerType.CurrentFrame);
        private Queue<KeyValuePair<FileOperation, InvokeType>> invokeQueue = new Queue<KeyValuePair<FileOperation, InvokeType>>();

        private Task invokerTask;
        private CancellationTokenSource cancelSource;

        [Inject] private DiContainer container;

        private enum InvokeType
        {
            Run,
            Undo
        }

        public FileOperator()
        {
            cancelSource = new CancellationTokenSource();
            invokerTask = new Task(InvokationLoop, cancelSource.Token);
            invokerTask.Start();
        }
        ~FileOperator()
        {
            cancelSource.Cancel();
        }

        public async void Run(FileOperation op)
        {
            container.Inject(op);
            await op.Run();
            history.Add(op);
            onAnyOperationApplied?.Invoke();
        }
        public async void Undo()
        {
            if (history.TryUndo(out FileOperation op))
            {
                await op.Undo();
                onAnyOperationApplied?.Invoke();
            }
        }
        public async void Redo()
        {
            if (history.TryRedo(out FileOperation op))
            {
                await op.Run();
                onAnyOperationApplied?.Invoke();
            }
        }

        private void AddToQueue(FileOperation op, InvokeType type)
        {
            invokeQueue.Enqueue(new KeyValuePair<FileOperation, InvokeType>(op, type));
        }
        private void InvokationLoop()
        {
            while (true)
            {
                while(invokeQueue.Count > 0)
                {
                    KeyValuePair<FileOperation, InvokeType> kv = invokeQueue.Dequeue();

                    if (kv.Value == InvokeType.Run) kv.Key.Run().Wait();
                    else kv.Key.Undo().Wait();
                }
                Task.Yield();
            }
        }
    }
}