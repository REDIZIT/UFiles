using System;
using Zenject;

namespace InApp
{
    public class FileOperator
    {
        public Action onAnyOperationApplied;

        private History<FileOperation> history = new History<FileOperation>(HistoryPointerType.CurrentFrame);

        [Inject] private DiContainer container;

        private enum InvokeType
        {
            Run,
            Undo
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
    }
}