using Avalonia.Platform.Storage;
using System;
using System.Threading.Tasks;

namespace FileObjectExtractor.Services
{
    public interface IWindowService
    {
        void CloseWindow();
        Task<IStorageFile?> OpenFileAsync(string title);
        Task<IStorageFile?> SaveFileAsync(string title, string suggestedFileName);
        Task<string?> SelectFolderAsync(string title);
        void ShowAboutWindow();
        void ShowErrorWindow(Exception ex);
        void ShowLicenseWindow();
    }
}
