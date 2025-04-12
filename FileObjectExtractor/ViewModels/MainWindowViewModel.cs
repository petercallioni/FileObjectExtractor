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
        private ProgressIndicatorViewModel progressIndicator;
        private ProgressService progressService;
        private IBackgroundExecutor backgroundExecutor;
        private bool trustToOpenFiles;

        private ObservableCollection<ExtractedFileViewModel> extractedFiles;
        private string filter;
        private bool isLoadingFile;
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

        private IFileController fileController;

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

        public bool HasSelectedItems
        {
            get => ExtractedFiles
                .Any(x => x.IsSelected);
        }

        public bool SelectedFilesContainWarnings
        {
            get => ExtractedFiles
                .Where(x => x.IsSelected)
                .Any(x => x.ExtractedFile.FileNameWarnings.Count > 0);
        }
        public ProgressIndicatorViewModel ProgressIndicator
        {
            get => progressIndicator; set
            {
                progressIndicator = value;
                OnPropertyChanged();
            }
        }

        public bool IsLoadingFile
        {
            get => isLoadingFile; set
            {
                isLoadingFile = value;
                OnPropertyChanged();
            }
        }

        public bool TrustToOpenFiles
        {
            get => trustToOpenFiles;
            set
            {
                trustToOpenFiles = value;
                OnPropertyChanged();
            }
        }

        public MainWindowViewModel(IFileController fileController, IWindowService windowService, IBackgroundExecutor backgroundExecutor) : base(windowService)
        {
            // VMs
            mainMenu = new MainMenuViewModel(windowService);
            progressIndicator = new ProgressIndicatorViewModel();
            progressService = new ProgressService(ProgressIndicator);

            // Initialisers
            inputFile = new InputFileViewModel();
            isLoadingFile = false;
            sortOrder = SortOrder.DOCUMENT;
            extractedFiles = new ObservableCollection<ExtractedFileViewModel>();
            filter = string.Empty;
            trustToOpenFiles = false;
            this.fileController = fileController;
            this.backgroundExecutor = backgroundExecutor;

            // Commands
            SelectAllCommand = new RelayCommand(SelectAll);
            SelectNoneCommand = new RelayCommand(SelectNone);
            SelectFileCommand = new RelayCommand(SelectFile);
            SaveSelectedCommand = new RelayCommand(SaveSelectedFiles);
            SelectSortCommand = new RelayCommand<SortOrder>(SelectSort);
        }

        private void ProcessInputFile(Uri uri)
        {
            InputFileViewModel? inputFile = null;
            if (uri != null && uri.IsAbsoluteUri)
            {
                IsLoadingFile = true;

                ExceptionSafe(() =>
                {
                    backgroundExecutor.Execute(() =>
                    {
                        IParseOffice parseOffice = OfficeParserPicker.GetOfficeParser(uri);

                        inputFile = new InputFileViewModel(uri, parseOffice.OfficeType);
                        List<ExtractedFile> embeddedFiles = parseOffice.GetExtractedFiles(uri).ToList();

                        ExtractedFiles.Clear();

                        foreach (ExtractedFile file in embeddedFiles)
                        {
                            ExtractedFileViewModel extractedFileVM = new ExtractedFileViewModel(file, SaveFile, OpenFile);

                            // Update the UI when the IsSelected property changes
                            extractedFileVM.PropertyChanged += (sender, e) =>
                            {
                                if (e.PropertyName == nameof(ExtractedFileViewModel.IsSelected))
                                {
                                    OnPropertyChanged(nameof(HasSelectedItems));
                                    OnPropertyChanged(nameof(SelectedFilesContainWarnings));
                                }
                            };

                            ExtractedFiles.Add(extractedFileVM);
                        }

                        return () =>
                        {
                            InputFile = inputFile;
                            IsLoadingFile = false;
                            ApplyFilter();
                            SortExtractedFiles(ExtractedFiles);
                        };
                    });
                },
                () =>
                {
                    IsLoadingFile = false;
                });
            }
        }

        public void SelectFile(Uri filePath)
        {
            ExceptionSafe(() =>
            {
                ProcessInputFile(filePath);
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
                    Avalonia.Platform.Storage.IStorageFile? file = await fileController.AskOpenFileAsync();

                    if (file != null)
                    {
                        ProcessInputFile(file.Path);
                    }
                });
        }

        private async void SaveFile(ExtractedFileViewModel extractedFileVM)
        {
            await ExceptionSafeAsync(async () =>
            {
                bool saveSuccess = await fileController.AskSaveFileAsync(extractedFileVM.ExtractedFile, progressService);
            });
        }

        private void OpenFile(ExtractedFileViewModel extractedFileVM)
        {
            Action openFileAction = () =>
                ExceptionSafe(() =>
                {
                    fileController.OpenFile(extractedFileVM.ExtractedFile);
                    TrustToOpenFiles = true;
                });

            if (!TrustToOpenFiles)
            {
                OpenFileTrustWindow(openFileAction);
            }
            else
            {
                openFileAction();
            }
        }

        private async void SaveSelectedFiles()
        {

            await ExceptionSafeAsync(async () =>
            {
                bool saveSuccess = await fileController.AskSaveMultipleFiles(
                    ExtractedFiles
                    .Where(x => x.IsSelected)
                    .Select(x => x.ExtractedFile
                ).ToList(), progressService);
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
                SortOrder.SIZE => new ObservableCollection<ExtractedFileViewModel>(files.OrderBy(x => x.ExtractedFile.EmbeddedFile.Length)),
                SortOrder.SIZE_DESC => new ObservableCollection<ExtractedFileViewModel>(files.OrderByDescending(x => x.ExtractedFile.EmbeddedFile.Length)),
                _ => files,
            };

            ExtractedFiles = sortedFiles;
        }

        private void OpenFileTrustWindow(Action confirmAction)
        {
            windowService.ShowFileTrustWindow(confirmAction);
        }
    }
}