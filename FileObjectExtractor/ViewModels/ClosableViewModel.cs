using FileObjectExtractor.Services;

namespace FileObjectExtractor.ViewModels
{
    public abstract class ClosableViewModel : ViewModelBase
    {
        protected IWindowService windowService;

        protected ClosableViewModel(IWindowService windowService)
        {
            this.windowService = windowService;
        }

        protected void Close()
        {
            windowService.CloseWindow();
        }
    }
}