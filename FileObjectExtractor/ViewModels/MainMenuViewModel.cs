using CommunityToolkit.Mvvm.Input;
using FileObjectExtractor.Services;

namespace FileObjectExtractor.ViewModels
{
    public class MainMenuViewModel : ViewModelBase
    {
        public IRelayCommand ExitCommand => new RelayCommand(Exit);
        public IRelayCommand AboutCommand => new RelayCommand(About);

        public MainMenuViewModel(IWindowService windowService) : base(windowService)
        {

        }

        private void Exit()
        {
            WindowService.CloseWindow();
        }

        private void About()
        {
            WindowService.ShowAboutWindow();
        }
    }
}