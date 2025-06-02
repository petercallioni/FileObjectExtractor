using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.Input;
using FileObjectExtractor.Services;
using FileObjectExtractor.Updates;
using FileObjectExtractor.ViewModels.Interfaces;
using System;
using System.Threading.Tasks;

namespace FileObjectExtractor.ViewModels.Design
{
    public class DesignMainMenuViewModel : MainMenuViewModel
    {
        private class DesignWindowService : IWindowService
        {
            public void CloseWindow() { }
            public void ShowAboutWindow() { }
            public void ShowUpdatesWindow(IUpdateService updateService, Update? update) { }
            public void ShowErrorWindow(Exception exception) { }

            public Task<IStorageFile?> OpenFileAsync(string title)
            {
                throw new NotImplementedException();
            }

            public Task<IStorageFile?> SaveFileAsync(string title, string suggestedFileName)
            {
                throw new NotImplementedException();
            }

            public Task<string?> SelectFolderAsync(string title)
            {
                throw new NotImplementedException();
            }

            public void ShowFileTrustWindow(Action confirmAction)
            {
                throw new NotImplementedException();
            }

            public void ShowUpdatesWindow(IUpdateService updateService)
            {
                throw new NotImplementedException();
            }
        }

        private class DesignUpdatesViewModel : IUpdatesViewModel
        {
            public bool HasUpdate => true;
            public string NewVersion => "v1.0.0";
            public Update? Update => null;
            public string UpdateTooltip => "Update available";
            public IRelayCommand CheckForUpdatesCommand => null!;

            bool IUpdatesViewModel.HasUpdate { get => HasUpdate; set => throw new NotImplementedException(); }
            string IUpdatesViewModel.NewVersion { get => NewVersion; set => throw new NotImplementedException(); }

            IAsyncRelayCommand IUpdatesViewModel.CheckForUpdatesCommand => throw new NotImplementedException();
        }

        private class DesignMainViewItemSelection : IMainViewItemSelection
        {
            public void SelectAll() { }
            public void SelectNone() { }
            public Task SaveSelectedFiles() => Task.CompletedTask;
            public Task SelectFile() => Task.CompletedTask;
        }

        public DesignMainMenuViewModel() : base(
            new DesignWindowService(),
            new DesignUpdatesViewModel(),
            new DesignMainViewItemSelection())
        {
        }
    }
}
