using System.Threading.Tasks;

namespace InApp
{
    public abstract class FileOperation
    {
        public abstract Task Run();
        public abstract Task Undo();
    }
}