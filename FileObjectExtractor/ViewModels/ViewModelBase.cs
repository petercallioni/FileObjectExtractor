using CommunityToolkit.Mvvm.ComponentModel;
using FileObjectExtractor.Services;
using System;
using System.Threading.Tasks;

namespace FileObjectExtractor.ViewModels
{
    public class ViewModelBase : ObservableObject
    {
        private IWindowService windowService;
        /// <summary>
        /// The constructor for the ViewModelBase class. 
        /// WindowService is optional, so it can be null if the ViewModel does not need it.
        /// </summary>
        /// <param name="windowService"></param>
        public ViewModelBase(IWindowService windowService)
        {
            this.windowService = windowService;
        }

        protected IWindowService WindowService { get => windowService; }

        protected void ExceptionSafe(Action action, Action? rollback = null)
        {
            try
            {
                action.Invoke();
            }
            catch (Exception ex)
            {
                HandleException(ex, rollback);
            }
        }

        protected async Task ExceptionSafeAsync(Func<Task> actionAsync, Action? rollback = null)
        {
            try
            {
                await actionAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                HandleException(ex, rollback);
            }
        }

        private void HandleException(Exception ex, Action? rollback)
        {
            rollback?.Invoke();
            windowService.ShowErrorWindow(ex);
        }
    }
}