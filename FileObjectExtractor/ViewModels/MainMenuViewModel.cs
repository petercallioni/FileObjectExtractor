using CommunityToolkit.Mvvm.Input;
using FileObjectExtractor.Models.ApplicationOptions;
using FileObjectExtractor.Services;
using FileObjectExtractor.Updates;
using FileObjectExtractor.ViewModels.Interfaces;

namespace FileObjectExtractor.ViewModels
{
    public class MainMenuViewModel : ViewModelBase
    {
        private readonly IWindowService windowService;
        private readonly IMainViewItemSelection mainViewItemSelection;
        private readonly IUpdatesViewModel updatesViewModel;

        public IRelayCommand ExitCommand => new RelayCommand(Exit);
        public IRelayCommand AboutCommand => new RelayCommand(About);
        public IRelayCommand SelectAllCommand => new RelayCommand(mainViewItemSelection.SelectAll);
        public IRelayCommand SelectNoneCommand => new RelayCommand(mainViewItemSelection.SelectNone);
        public IAsyncRelayCommand SaveSelectedFilesCommand => new AsyncRelayCommand(mainViewItemSelection.SaveSelectedFiles);
        public IAsyncRelayCommand SelectFileCommand => new AsyncRelayCommand(mainViewItemSelection.SelectFile);
        public IRelayCommand OpenUpdatesWindowCommand => new RelayCommand(OpenUpdatesWindow);

        public bool CheckForUpdatesOnStartup
        {
            get => ApplicationOptionsManager.Options.CheckForUpdateOnStartup;
            set
            {
                ApplicationOptionsManager.Options.CheckForUpdateOnStartup = value;
                OnPropertyChanged();
            }
        }

        public MainMenuViewModel(IWindowService windowService, IUpdatesViewModel updatesViewModel, IMainViewItemSelection mainViewItemSelection) : base(windowService)
        {
            this.windowService = windowService;
            this.mainViewItemSelection = mainViewItemSelection;
            this.updatesViewModel = updatesViewModel; // This what this class should use for updates functionality     
        }

        private void OpenUpdatesWindow()
        {
            windowService.ShowUpdatesWindow(new UpdateService(), UpdatesViewModel.Update);
        }

        private void Exit()
        {
            windowService.CloseWindow();
        }

        private void About()
        {
            windowService.ShowAboutWindow();
        }

        public IUpdatesViewModel UpdatesViewModel => updatesViewModel;
    }
}