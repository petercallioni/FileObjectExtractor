using FileObjectExtractor.ViewModels;

namespace FileObjectExtractor.Services
{
    public class ProgressService : IProgressService
    {
        private ProgressIndicatorViewModel progressIndicatorViewModel;
        public ProgressService(ProgressIndicatorViewModel progressIndicatorViewModel)
        {
            this.progressIndicatorViewModel = progressIndicatorViewModel;
        }

        public void ShowProgress()
        {
            Avalonia.Threading.Dispatcher.UIThread.Post(() =>
            {
                progressIndicatorViewModel.ShowProgress = true;
            });
        }

        public void HideProgress()
        {
            Avalonia.Threading.Dispatcher.UIThread.Post(() =>
            {
                progressIndicatorViewModel.ShowProgress = false;
            });
        }

        public void SetProgress(int progress)
        {
            Avalonia.Threading.Dispatcher.UIThread.Post(() =>
            {
                progressIndicatorViewModel.Progress = progress;
            });
        }

        public void SetMaximum(int maximum)
        {
            Avalonia.Threading.Dispatcher.UIThread.Post(() =>
            {
                progressIndicatorViewModel.MaxProgress = maximum;
            });
        }

        public void SetMessage(string message)
        {
            Avalonia.Threading.Dispatcher.UIThread.Post(() =>
            {
                progressIndicatorViewModel.Message = message;
            });
        }
    }
}
