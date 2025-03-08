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
        public IRelayCommand SelectSortCommand { get; init; }

        private SortOrder sortOrder;
        public ObservableCollection<ExtractedFileViewModel> ExtractedFiles
        {
            get => extractedFiles; set
            {
                extractedFiles = value;
                OnPropertyChanged();
            }
        }

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

        public SortOrder SortOrder
        {
            get => sortOrder; set
            {
                sortOrder = value;
                OnPropertyChanged();
            }
        }

        public bool SelectedFilesContainWarnings
        {
            get => ExtractedFiles
                .Where(x => x.IsSelected)
                .Any(x => x.ExtractedFile.FileNameWarnings.Count > 0);
        }

        public MainWindowViewModel(FileController fileController, IWindowService windowService) : base(windowService)
        {
            mainMenu = new MainMenuViewModel(windowService);

            sortOrder = SortOrder.DOCUMENT;
            inputFile = new InputFileViewModel();
            extractedFiles = new ObservableCollection<ExtractedFileViewModel>();
            filter = string.Empty;
            this.fileController = fileController;

            SelectAllCommand = new RelayCommand(SelectAll);
            SelectNoneCommand = new RelayCommand(SelectNone);
            SelectFileCommand = new RelayCommand(SelectFile);
            SaveSelectedCommand = new RelayCommand(SaveSelectedFiles);
            SelectSortCommand = new RelayCommand<SortOrder>(SelectSort);
        }

        private void ProcessInputFile(InputFileViewModel inputFile)
        {
            if (inputFile.FileURI != null && inputFile.FileURI.IsAbsoluteUri)
            {
                IParseOffice parseOffice = OfficeParserPicker.GetOfficeParser(inputFile.FileURI);
                inputFile.OfficeType = parseOffice.OfficeType;

                List<ExtractedFile> embeddedFiles = parseOffice.GetExtractedFiles(inputFile.FileURI).ToList();

                ExtractedFiles.Clear();

                foreach (ExtractedFile file in embeddedFiles)
                {
                    ExtractedFileViewModel extractedFileVM = new ExtractedFileViewModel(file, SaveFile);

                    // Update the UI when the IsSelected property changes
                    extractedFileVM.PropertyChanged += (sender, e) =>
                    {
                        if (e.PropertyName == nameof(ExtractedFileViewModel.IsSelected))
                        {
                            OnPropertyChanged(nameof(SelectedFilesContainWarnings));
                        }
                    };

                    ExtractedFiles.Add(extractedFileVM);
                }

                ApplyFilter();
                SortExtractedFiles(ExtractedFiles);
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

        private void SelectSort(SortOrder sort)
        {
            SortOrder = sort;

            SortExtractedFiles(ExtractedFiles);
        }

        private void SortExtractedFiles(ObservableCollection<ExtractedFileViewModel> files)
        {
            ObservableCollection<ExtractedFileViewModel> sortedFiles = sortOrder switch
            {
                SortOrder.DOCUMENT => new ObservableCollection<ExtractedFileViewModel>(files.OrderBy(x => x.ExtractedFile.DocumentOrder)),
                SortOrder.DOCUMENT_DESC => new ObservableCollection<ExtractedFileViewModel>(files.OrderByDescending(x => x.ExtractedFile.DocumentOrder)),
                SortOrder.ALPHABETICAL => new ObservableCollection<ExtractedFileViewModel>(files.OrderBy(x => x.ExtractedFile.FileName)),
                SortOrder.ALPHABETICAL_DESC => new ObservableCollection<ExtractedFileViewModel>(files.OrderByDescending(x => x.ExtractedFile.FileName)),
                _ => files,
            };

            ExtractedFiles = sortedFiles;
        }
    }
}