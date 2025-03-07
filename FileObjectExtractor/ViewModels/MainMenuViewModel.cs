using CommunityToolkit.Mvvm.Input;
using FileObjectExtractor.Services;

namespace FileObjectExtractor.ViewModels
{
    public class MainMenuViewModel : ViewModelBase
    {
        private IWindowService windowService;
        public IRelayCommand ExitCommand => new RelayCommand(Exit);
        public IRelayCommand AboutCommand => new RelayCommand(About);

        public MainMenuViewModel(IWindowService windowService)
        {
            this.windowService = windowService;
        }

        private void Exit()
        {
            windowService.CloseWindow();
        }

        private void About()
        {
            windowService.ShowAboutWindow();
        }
    }
}