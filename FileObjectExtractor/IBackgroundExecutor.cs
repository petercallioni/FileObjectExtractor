using System;
using System.Threading.Tasks;

namespace FileObjectExtractor
{
    public interface IBackgroundExecutor
    {
        Task ExecuteAsync(Action action);
        Task ExecuteAsync(Func<Action> function);
    }
}
