using Avalonia.Controls;
using Avalonia.Platform.Storage;
using FileObjectExtractor.ViewModels;
using FileObjectExtractor.Views;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FileObjectExtractor.Services
{
    public class WindowService : IWindowService
    {
        private readonly Window window;

        public WindowService(Window window)
        {
            this.window = window;
        }

        public void ShowErrorWindow(Exception ex)
        {
            Avalonia.Threading.Dispatcher.UIThread.Post(() =>
            {
                ErrorWindow errorWindow = new ErrorWindow(window);
                ErrorWindowViewModel errorWindowViewModel = new ErrorWindowViewModel(ex, new WindowService(errorWindow));
                errorWindow.DataContext = errorWindowViewModel;
                errorWindow.ShowDialog(window);
            });
        }

        public void ShowAboutWindow()
        {
            Avalonia.Threading.Dispatcher.UIThread.Post(() =>
            {
                AboutWindow aboutWindow = new AboutWindow(window);
                AboutViewModel aboutViewModel = new AboutViewModel(new WindowService(aboutWindow));
                aboutWindow.DataContext = aboutViewModel;
                aboutWindow.ShowDialog(window);
            });
        }

        public void ShowLicenseWindow()
        {
            Avalonia.Threading.Dispatcher.UIThread.Post(() =>
            {
                LicenseWindow LicenseWindow = new LicenseWindow(window);
                LicenseViewModel LicenseViewModel = new LicenseViewModel(new WindowService(LicenseWindow));
                LicenseWindow.DataContext = LicenseViewModel;
                LicenseWindow.ShowDialog(window);
            });
        }

        public async Task<IStorageFile?> OpenFileAsync(string title)
        {
            IReadOnlyList<IStorageFile> files = await window.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
            {
                Title = title,
                AllowMultiple = false
            });

            IEnumerator<IStorageFile> enumerator = files.GetEnumerator();
            return enumerator.MoveNext() ? enumerator.Current : null;
        }

        public async Task<string?> SelectFolderAsync(string title)
        {
            IReadOnlyList<IStorageFolder> selectedFolders = await window.StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
            {
                Title = title
            });

            IEnumerator<IStorageFolder> enumerator = selectedFolders.GetEnumerator();
            return enumerator.MoveNext() ? enumerator.Current?.Path.AbsolutePath : null;
        }

        public async Task<IStorageFile?> SaveFileAsync(string title, string suggestedFileName)
        {
            FilePickerSaveOptions saveOptions = new()
            {
                Title = title,
                SuggestedFileName = suggestedFileName
            };

            return await window.StorageProvider.SaveFilePickerAsync(saveOptions);
        }

        public void CloseWindow()
        {
            window.Close();
        }
    }
}
