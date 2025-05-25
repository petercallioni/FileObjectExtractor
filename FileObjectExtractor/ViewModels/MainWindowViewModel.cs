using CommunityToolkit.Mvvm.Input;
using ExCSS;
using FileObjectExtractor.Models;
using FileObjectExtractor.Models.Office;
using FileObjectExtractor.Services;
using FileObjectExtractor.Updates;
using FileObjectExtractor.ViewModels.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace FileObjectExtractor.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBase, IMainViewItemSelection
    {
        // Sub ViewModels
        private InputFileViewModel inputFile;
        private MainMenuViewModel mainMenu;
        private ProgressIndicatorViewModel progressIndicator;
        private TemporaryFilesViewModel temporaryFilesViewModel;
        private UpdatesViewModel updatesViewModel;

        private ProgressService progressService;
        private IBackgroundExecutor backgroundExecutor;
        private bool trustToOpenFiles;

        private List<ExtractedFileViewModel> extractedFiles;
        private ObservableCollection<ExtractedFileViewModel> filteredExtractedFiles;

        private string filter;
        private bool isLoadingFile;

        public IRelayCommand SelectAllCommand { get; init; }
        public IRelayCommand SelectNoneCommand { get; init; }
        public IRelayCommand SaveSelectedCommand { get; init; }
        public IRelayCommand SelectFileCommand { get; init; }
        public IRelayCommand SelectSortCommand { get; init; }

        private SortOrder sortOrder;
        public List<ExtractedFileViewModel> ExtractedFiles
        {
            get => extractedFiles; set
            {
                extractedFiles = value;
            }
        }

        public ObservableCollection<ExtractedFileViewModel> FilteredExtractedFiles
        {
            get => filteredExtractedFiles; set
            {
                filteredExtractedFiles = value;
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

        public TemporaryFilesViewModel TemporaryFilesViewModel
        {
            get => temporaryFilesViewModel;
            set
            {
                temporaryFilesViewModel = value;
                OnPropertyChanged();
            }
        }

        public UpdatesViewModel UpdatesViewModel
        {
            get => updatesViewModel;
            set
            {
                updatesViewModel = value;
                OnPropertyChanged();
            }
        }

        public MainWindowViewModel(IFileController fileController, IWindowService windowService, IUpdateService updateService, IBackgroundExecutor backgroundExecutor) : base(windowService)
        {
            // VMs
            progressIndicator = new ProgressIndicatorViewModel();
            temporaryFilesViewModel = new TemporaryFilesViewModel(windowService);
            updatesViewModel = new UpdatesViewModel(windowService, updateService);
            mainMenu = new MainMenuViewModel(windowService, updatesViewModel, this); // This is ugly, and is hard to refactor

            // Services
            progressService = new ProgressService(ProgressIndicator);

            // Initialisers
            inputFile = new InputFileViewModel();
            isLoadingFile = false;
            sortOrder = SortOrder.DOCUMENT;
            extractedFiles = new List<ExtractedFileViewModel>();
            filteredExtractedFiles = new ObservableCollection<ExtractedFileViewModel>();
            filter = string.Empty;
            trustToOpenFiles = false;
            this.fileController = fileController;
            this.backgroundExecutor = backgroundExecutor;

            // Commands
            SelectAllCommand = new RelayCommand(SelectAll);
            SelectNoneCommand = new RelayCommand(SelectNone);
            SelectFileCommand = new AsyncRelayCommand(SelectFile);
            SaveSelectedCommand = new AsyncRelayCommand(SaveSelectedFiles);
            SelectSortCommand = new RelayCommand<SortOrder>(SelectSort);
        }

        private async void ProcessInputFile(Uri uri)
        {
            InputFileViewModel? inputFile = null;
            if (uri != null && uri.IsAbsoluteUri)
            {
                IsLoadingFile = true;

                // This is incredibly ugly and should be refactored
                await ExceptionSafeAsync(async () =>
                {
                    await backgroundExecutor.ExecuteAsync(() =>
                     {
                         IParseOffice parseOffice = OfficeParserPicker.GetOfficeParser(uri);

                         inputFile = new InputFileViewModel(uri, parseOffice.OfficeType);
                         List<ExtractedFile> embeddedFiles = parseOffice.GetExtractedFiles(uri).ToList();

                         ExtractedFiles.Clear();

                         foreach (ExtractedFile file in embeddedFiles)
                         {
                             ExtractedFileViewModel extractedFileVM = new ExtractedFileViewModel(WindowService, file, SaveFile, OpenFile);

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
                             SortExtractedFiles(FilteredExtractedFiles);
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
                if (item.IsVisible)
                    item.IsSelected = true;
            }
        }

        public void SelectNone()
        {
            foreach (ExtractedFileViewModel item in ExtractedFiles)
            {
                if (item.IsVisible)
                    item.IsSelected = false;
            }
        }
        public async Task SaveSelectedFiles()
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

        public async Task SelectFile()
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

        private void UpdateFilteredCollection()
        {
            List<ExtractedFileViewModel> filtered = extractedFiles.Where(x => x.IsVisible).ToList();

            FilteredExtractedFiles.Clear();
            foreach (ExtractedFileViewModel item in filtered)
            {
                FilteredExtractedFiles.Add(item);
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
                        item.IsVisible = item.ExtractedFile.FileName.Contains(filter, System.StringComparison.OrdinalIgnoreCase);
                    }
                    else
                    {
                        item.IsVisible = true;
                    }
                }
                UpdateFilteredCollection();
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
                ExceptionSafe((Action)(async () =>
                {
                    extractedFileVM.CanOpen = false;
                    TrustToOpenFiles = true;
                    await fileController.OpenFile(extractedFileVM.ExtractedFile);
                    temporaryFilesViewModel.RefreshTemporaryFilesInfo();
                    extractedFileVM.CanOpen = true;
                }));

            if (!TrustToOpenFiles)
            {
                OpenFileTrustWindow(openFileAction);
            }
            else
            {
                openFileAction();
            }
        }

        private void SelectSort(SortOrder sort)
        {
            SortOrder = sort;

            SortExtractedFiles(FilteredExtractedFiles);
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

            FilteredExtractedFiles = sortedFiles;
        }

        private void OpenFileTrustWindow(Action confirmAction)
        {
            WindowService.ShowFileTrustWindow(confirmAction);
        }
    }
}