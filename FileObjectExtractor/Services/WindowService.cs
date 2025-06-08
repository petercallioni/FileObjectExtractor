using Avalonia.Controls;
using Avalonia.Platform.Storage;
using FileObjectExtractor.Extensions;
using FileObjectExtractor.Models.Office;
using FileObjectExtractor.Updates;
using FileObjectExtractor.ViewModels;
using FileObjectExtractor.Views;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public void ShowFileTrustWindow(Action confirmAction)
        {
            Avalonia.Threading.Dispatcher.UIThread.Post(() =>
            {
                FileTrustWindow fileTrustWindow = new FileTrustWindow(window);
                FileTrustWindowViewModel fileTrustViewModel = new FileTrustWindowViewModel(new WindowService(fileTrustWindow), confirmAction);
                fileTrustWindow.DataContext = fileTrustViewModel;
                fileTrustWindow.ShowDialog(window);
            });
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

        public void ShowUpdatesWindow(IUpdateService updateService)
        {
            Avalonia.Threading.Dispatcher.UIThread.Post(() =>
            {
                UpdatesWindow updatesWindow = new UpdatesWindow(window);
                UpdatesViewModel updatesViewModel = new UpdatesViewModel(new WindowService(updatesWindow), updateService);
                updatesWindow.DataContext = updatesViewModel;
                updatesWindow.ShowDialog(window);
            });
        }

        public void ShowUpdatesWindow(IUpdateService updateService, Update? update)
        {
            Avalonia.Threading.Dispatcher.UIThread.Post(() =>
            {
                UpdatesWindow updatesWindow = new UpdatesWindow(window);
                UpdatesViewModel updatesViewModel = new UpdatesViewModel(new WindowService(updatesWindow), updateService, update);
                updatesWindow.DataContext = updatesViewModel;
                updatesWindow.ShowDialog(window);
            });
        }

        private static readonly FilePickerFileType[] OfficeFileTypes = new FilePickerFileType[]
        {
            new FilePickerFileType("All Office Files")
            {
                Patterns = OfficeParserPicker.ALL_EXTENSIONS.Select(ext => $"*{ext}").ToList()
            },
            new FilePickerFileType("Word Files")
            {
                Patterns = OfficeParserPicker.WORD_EXTENSIONS.Select(ext => $"*{ext}").ToList()
            },
            new FilePickerFileType("Excel Files")
            {
                Patterns = OfficeParserPicker.EXCEL_EXTENSIONS.Select(ext => $"*{ext}").ToList()
            },
            new FilePickerFileType("PowerPoint Files")
            {
                Patterns = OfficeParserPicker.POWERPOINT_EXTENSIONS.Select(ext => $"*{ext}").ToList()
            }
        };

        public async Task<IStorageFile?> OpenFileAsync(string title)
        {
            IReadOnlyList<IStorageFile> files = await window.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
            {
                Title = title,
                AllowMultiple = false,
                FileTypeFilter = OfficeFileTypes
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
            return enumerator.MoveNext() ? enumerator.Current?.Path.UnescapedString() : null;
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
