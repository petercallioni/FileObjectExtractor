using Avalonia.Platform.Storage;
using FileObjectExtractor.Services;
using FileObjectExtractor.Updates;
using System;
using System.Threading.Tasks;

namespace FileObjectExtractor.ViewModels.Design
{
    public class MockWindowService : IWindowService
    {
        public void CloseWindow()
        {
            // This method is intentionally left empty for mock purposes.
        }

        public Task<IStorageFile?> OpenFileAsync(string title)
        {
            // This method is intentionally left empty for mock purposes.
            return null!;
        }

        public Task<IStorageFile?> SaveFileAsync(string title, string suggestedFileName)
        {
            // This method is intentionally left empty for mock purposes.
            return null!;
        }

        public Task<string?> SelectFolderAsync(string title)
        {
            // This method is intentionally left empty for mock purposes.
            return null!;
        }

        public void ShowAboutWindow()
        {
            // This method is intentionally left empty for mock purposes.
        }

        public void ShowErrorWindow(Exception ex)
        {
            // This method is intentionally left empty for mock purposes.
        }

        public void ShowFileTrustWindow(Action confirmAction)
        {
            // This method is intentionally left empty for mock purposes.
        }


        public void ShowUpdatesWindow(IUpdateService updateService)
        {
            // This method is intentionally left empty for mock purposes.
        }

        public void ShowUpdatesWindow(IUpdateService updateService, Update? update)
        {
            // This method is intentionally left empty for mock purposes.
        }

        Task<IStorageFile?> IWindowService.OpenFileAsync(string title)
        {
            throw new NotImplementedException();
        }

        Task<IStorageFile?> IWindowService.SaveFileAsync(string title, string suggestedFileName)
        {
            throw new NotImplementedException();
        }

        Task<string?> IWindowService.SelectFolderAsync(string title)
        {
            throw new NotImplementedException();
        }
    }
}
