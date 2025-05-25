using CommunityToolkit.Mvvm.Input;
using FileObjectExtractor.Services;

namespace FileObjectExtractor.ViewModels
{
    public abstract class ClosableViewModel : ViewModelBase
    {
        public IRelayCommand CloseCommand => new RelayCommand(Close);

        protected ClosableViewModel(IWindowService windowService) : base(windowService)
        {

        }

        protected void Close()
        {
            WindowService.CloseWindow();
        }
    }
}