using System;

namespace InApp
{
    public class FileOperator
    {
        public Action onAnyOperationApplied;

        private History<FileOperation> history = new History<FileOperation>(HistoryPointerType.CurrentFrame);

        public void Run(FileOperation op)
        {
            op.Run();
            history.Add(op);
            onAnyOperationApplied?.Invoke();
        }
        public void Undo()
        {
            if (history.TryUndo(out FileOperation op))
            {
                op.Undo();
                onAnyOperationApplied?.Invoke();
            }
        }
        public void Redo()
        {
            if (history.TryRedo(out FileOperation op))
            {
                op.Run();
                onAnyOperationApplied?.Invoke();
            }
        }
    }
}