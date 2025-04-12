using System;

namespace FileObjectExtractor
{
    public interface IBackgroundExecutor
    {
        void Execute(Action action);
        void Execute(Func<Action> function);
    }
}
