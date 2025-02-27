using CommunityToolkit.Mvvm.Input;
using FileObjectExtractor.Models;
using FileObjectExtractor.Models.Office;
using FileObjectExtractor.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace FileObjectExtractor.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBase
    {
        private ObservableCollection<ExtractedFileViewModel> extractedFiles;
        private Uri? droppedFile;
        private string filter;
        private IWindowService windowService;

        public IRelayCommand ProcessCommand { get; init; }
        public IRelayCommand SelectAllCommand { get; init; }
        public IRelayCommand SelectNoneCommand { get; init; }
        public IRelayCommand SaveSelectedCommand { get; init; }
        public IRelayCommand SelectFileCommand { get; init; }
        public ObservableCollection<ExtractedFileViewModel> ExtractedFiles { get => extractedFiles; set => extractedFiles = value; }

        private FileController fileController;
        public Uri? DroppedFile
        {
            get => droppedFile; set
            {
                if (droppedFile != value)
                {
                    {
                        droppedFile = value;
                        OnPropertyChanged();
                    }
                }
            }
        }

        public string Filter
        {
            get => filter; set
            {
                if (filter != value)
                {
                    {
                        filter = value;
                        ApplyFilter();
                        OnPropertyChanged();
                    }
                }
            }
        }

        public MainWindowViewModel(FileController fileController, IWindowService windowService)
        {
            extractedFiles = new ObservableCollection<ExtractedFileViewModel>();
            filter = string.Empty;
            droppedFile = null;
            ProcessCommand = new RelayCommand(ProcessSelectedItems);
            SelectAllCommand = new RelayCommand(SelectAll);
            SelectNoneCommand = new RelayCommand(SelectNone);
            SelectFileCommand = new RelayCommand(SelectFile);
            SaveSelectedCommand = new RelayCommand(SaveSelectedFiles);
            this.fileController = fileController;
            this.windowService = windowService;
        }

        private void ProcessSelectedItems()
        {
            if (DroppedFile != null && DroppedFile.IsAbsoluteUri)
            {
                IParseOffice parseOffice = OfficeParserPicker.GetOfficeParser(DroppedFile);

                List<ExtractedFile> embeddedFiles = parseOffice.GetExtractedFiles(DroppedFile)
                    .OrderBy(x => x.FileName)
                    .ToList();

                ExtractedFiles.Clear();

                foreach (ExtractedFile file in embeddedFiles)
                {
                    ExtractedFileViewModel extractedFileVM = new ExtractedFileViewModel(file, SaveFile);
                    ExtractedFiles.Add(extractedFileVM);
                }

                ApplyFilter();
            }
        }

        public void SelectFile(Uri filePath)
        {
            Uri? previousFile = DroppedFile;
            ExceptionSafe(() =>
            {
                DroppedFile = filePath;
                ProcessSelectedItems();
            },
            () =>
            {
                DroppedFile = previousFile;
            });

        }

        public void SelectAll()
        {
            foreach (ExtractedFileViewModel item in ExtractedFiles)
            {
                item.IsSelected = true;
            }
        }

        public void SelectNone()
        {
            foreach (ExtractedFileViewModel item in ExtractedFiles)
            {
                item.IsSelected = false;
            }
        }

        private void ApplyFilter()
        {
            ExceptionSafe(() =>
            {
                foreach (ExtractedFileViewModel item in ExtractedFiles)
                {
                    if (filter.Length > 0)
                    {
                        item.IsSelected = item.ExtractedFile.FileName.Contains(filter, System.StringComparison.OrdinalIgnoreCase);
                    }
                    else
                    {
                        item.IsSelected = false;
                    }
                }
            });
        }

        private async void SelectFile()
        {
            Uri? previousFile = DroppedFile;

            await ExceptionSafeAsync(
                async () =>
                {
                    Avalonia.Platform.Storage.IStorageFile? file = await fileController.OpenFileAsync();

                    if (file != null)
                    {
                        DroppedFile = file.Path;
                        ProcessSelectedItems();
                    }
                },
                () =>
                {
                    DroppedFile = previousFile;
                });
        }

        private async void SaveFile(ExtractedFileViewModel extractedFileVM)
        {
            await ExceptionSafeAsync(async () =>
            {
                bool saveSuccess = await fileController.SaveFileAsync(extractedFileVM.ExtractedFile);
            });

        }

        private async void SaveSelectedFiles()
        {
            await ExceptionSafeAsync(async () =>
            {
                bool saveSuccess = await fileController.SaveMultipleFiles(
                    ExtractedFiles
                    .Where(x => x.IsSelected)
                    .Select(x => x.ExtractedFile
                ).ToList());
            });

        }

        private void ExceptionSafe(Action action, Action? rollback = null)
        {
            try
            {
                action.Invoke();
            }
            catch (Exception ex)
            {
                HandleException(ex, rollback);
            }
        }

        private async Task ExceptionSafeAsync(Func<Task> actionAsync, Action? rollback = null)
        {
            try
            {
                await actionAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                HandleException(ex, rollback);
            }
        }

        private void HandleException(Exception ex, Action? rollback)
        {
            rollback?.Invoke();
            windowService.ShowErrorWindow(ex);
        }
    }
}