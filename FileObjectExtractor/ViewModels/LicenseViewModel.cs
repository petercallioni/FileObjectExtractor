using CommunityToolkit.Mvvm.Input;
using FileObjectExtractor.Services;

namespace FileObjectExtractor.ViewModels
{
    public class LicenseViewModel : ClosableViewModel
    {
        public IRelayCommand CloseCommand => new RelayCommand(Close);
        public LicenseViewModel(IWindowService windowService) : base(windowService)
        {
        }
    }
}
