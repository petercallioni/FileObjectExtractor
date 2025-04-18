using System;
using System.Threading;
using System.Threading.Tasks;

namespace FileObjectExtractor
{
    public class BackgroundExecutor : IBackgroundExecutor
    {
        private readonly SynchronizationContext synchronizationContext;
        public BackgroundExecutor(SynchronizationContext synchronizationContext)
        {
            this.synchronizationContext = synchronizationContext;
        }

        // Executes an Action asynchronously and returns a Task.
        public Task ExecuteAsync(Action action)
        {
            // Task.Run will capture exceptions automatically.
            return Task.Run(() => action());
        }

        // Executes a function that returns a continuation Action.
        // It posts the continuation back to the SynchronizationContext.
        public async Task ExecuteAsync(Func<Action> function)
        {
            // Run the function asynchronously to get the continuation.
            Action continuation = await Task.Run(function);
            // Post the continuation to the SynchronizationContext.
            // Since Send is synchronous, exceptions in continuation() will be thrown here.
            synchronizationContext.Send(x => continuation(), null);
        }
    }
}