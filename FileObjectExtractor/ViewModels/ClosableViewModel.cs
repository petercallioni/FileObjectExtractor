using FileObjectExtractor.Services;

namespace FileObjectExtractor.ViewModels
{
    public abstract class ClosableViewModel : ViewModelBase
    {
        protected ClosableViewModel(IWindowService windowService) : base(windowService)
        {

        }

        protected void Close()
        {
            WindowService.CloseWindow();
        }
    }
}