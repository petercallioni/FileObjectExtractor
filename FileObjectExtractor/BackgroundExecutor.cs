using System;
using System.Threading;

namespace FileObjectExtractor
{
    public class BackgroundExecutor : IBackgroundExecutor
    {
        private readonly SynchronizationContext synchronizationContext;
        public BackgroundExecutor(SynchronizationContext
        synchronizationContext)
        {
            this.synchronizationContext = synchronizationContext;
        }
        public void Execute(Action action)
        {
            ThreadPool.QueueUserWorkItem(o => action());
        }
        public void Execute(Func<Action> function)
        {
            ThreadPool.QueueUserWorkItem(o =>
            {
                Action continuation = function();
                synchronizationContext.Send(x => continuation(), null);
            });
        }
    }
}
