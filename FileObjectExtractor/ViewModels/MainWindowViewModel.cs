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
    public partial class MainWindowViewModel : ClosableViewModel
    {
        private InputFileViewModel inputFile;
        private MainMenuViewModel mainMenu;

        private ObservableCollection<ExtractedFileViewModel> extractedFiles;
        private string filter;
        public IRelayCommand SelectAllCommand { get; init; }
        public IRelayCommand SelectNoneCommand { get; init; }
        public IRelayCommand SaveSelectedCommand { get; init; }
        public IRelayCommand SelectFileCommand { get; init; }
        public ObservableCollection<ExtractedFileViewModel> ExtractedFiles { get => extractedFiles; set => extractedFiles = value; }

        private FileController fileController;

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

        public InputFileViewModel InputFile
        {
            get => inputFile; set
            {
                inputFile = value;
                OnPropertyChanged();
            }
        }

        public MainMenuViewModel MainMenu
        {
            get => mainMenu; set
            {
                mainMenu = value;
                OnPropertyChanged();
            }
        }

        public MainWindowViewModel(FileController fileController, IWindowService windowService) : base(windowService)
        {
            mainMenu = new MainMenuViewModel(windowService);
            inputFile = new InputFileViewModel();
            extractedFiles = new ObservableCollection<ExtractedFileViewModel>();
            filter = string.Empty;
            SelectAllCommand = new RelayCommand(SelectAll);
            SelectNoneCommand = new RelayCommand(SelectNone);
            SelectFileCommand = new RelayCommand(SelectFile);
            SaveSelectedCommand = new RelayCommand(SaveSelectedFiles);
            this.fileController = fileController;
        }

        private void ProcessInputFile(InputFileViewModel inputFile)
        {
            if (inputFile.FileURI != null && inputFile.FileURI.IsAbsoluteUri)
            {
                IParseOffice parseOffice = OfficeParserPicker.GetOfficeParser(inputFile.FileURI);
                inputFile.OfficeType = parseOffice.OfficeType;

                List<ExtractedFile> embeddedFiles = parseOffice.GetExtractedFiles(inputFile.FileURI)
                    // .OrderBy(x => x.FileName)
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
            InputFileViewModel newFile = new InputFileViewModel(filePath);

            ExceptionSafe(() =>
            {
                ProcessInputFile(newFile);
                InputFile = newFile;
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
            await ExceptionSafeAsync(
                async () =>
                {
                    Avalonia.Platform.Storage.IStorageFile? file = await fileController.OpenFileAsync();

                    if (file != null)
                    {
                        InputFileViewModel newFilefile = new InputFileViewModel(file.Path);
                        ProcessInputFile(newFilefile);
                        InputFile = newFilefile;
                    }
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