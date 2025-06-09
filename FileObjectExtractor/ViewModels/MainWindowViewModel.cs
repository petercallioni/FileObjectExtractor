using CommunityToolkit.Mvvm.Input;
using ExCSS;
using FileObjectExtractor.Models;
using FileObjectExtractor.Models.ApplicationOptions;
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
        private Stack<NagivationState> navigationStack;

        private bool canGoBack;

        public IRelayCommand SelectAllCommand { get; init; }
        public IRelayCommand SelectNoneCommand { get; init; }
        public IRelayCommand SaveSelectedCommand { get; init; }
        public IRelayCommand SelectFileCommand { get; init; }
        public IRelayCommand SelectSortCommand { get; init; }
        public IRelayCommand GoBackCommand { get; init; }

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

        public Stack<NagivationState> NavigationStack { get => navigationStack; set => navigationStack = value; }

        public bool CanGoBack
        {
            get => canGoBack;
            set
            {
                canGoBack = value;
                OnPropertyChanged();
            }
        }

        private NagivationState currentNavigationState;

        public MainWindowViewModel(
            IFileController fileController,
            IWindowService windowService,
            IUpdateService updateService,
            IBackgroundExecutor backgroundExecutor
            ) : base(windowService)
        {
            // VMs
            progressIndicator = new ProgressIndicatorViewModel();
            temporaryFilesViewModel = new TemporaryFilesViewModel(windowService);

            updatesViewModel = new UpdatesViewModel(windowService, updateService);

            if (ApplicationOptionsManager.Options.CheckForUpdateOnStartup)
            {
                try
                {
                    updatesViewModel.CheckForUpdatesUnsafe();
                }
                catch (Exception ex)
                {
                    windowService.ShowErrorWindow(new AggregateException("Failed to check for updates.\nChecking for updates at start up has been disabled.", ex));
                    ApplicationOptionsManager.Options.CheckForUpdateOnStartup = false;
                }
            }

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
            currentNavigationState = new NagivationState(null, null);
            navigationStack = new Stack<NagivationState>();

            // Commands
            SelectAllCommand = new RelayCommand(SelectAll);
            SelectNoneCommand = new RelayCommand(SelectNone);
            SelectFileCommand = new AsyncRelayCommand(SelectFile);
            SaveSelectedCommand = new AsyncRelayCommand(SaveSelectedFiles);
            SelectSortCommand = new RelayCommand<SortOrder>(SelectSort);
            GoBackCommand = new RelayCommand(PopFromNaviagationStack);

            if (Global.StartedFromUpdate)
            {
                _ = UpdateService.CleanUpPostUpdate();
            }
        }

        /// <summary>
        /// Opens the specified file in the application and processes its contents.
        /// </summary>
        /// <remarks>This method updates the UI to reflect the contents of the file and manages navigation
        /// state. It ensures that the operation is performed safely and handles exceptions gracefully.</remarks>
        /// <param name="extractedFileVM">The view model representing the extracted file to be opened.  This parameter must not be <see
        /// langword="null"/>.</param>
        private async void OpenFileInFox(ExtractedFileViewModel extractedFileVM)
        {
            IsLoadingFile = true;
            await ExceptionSafeAsync(async () =>
            {
                await backgroundExecutor.ExecuteAsync(() =>
                {
                    return ProcessFileAndUpdateUI(
                        extractedFileVM.FileName,
                        () => OfficeParserPicker.GetOfficeParser(extractedFileVM.FileName)
                            .GetExtractedFiles(extractedFileVM.ExtractedFile.EmbeddedFile)
                            .ToList(),
                        true);
                });
            },
            () =>
            {
                IsLoadingFile = false;
                navigationStack.Pop();
                CanGoBack = navigationStack.Count > 1;
            });
        }

        /// <summary>
        /// Processes the specified input file asynchronously and updates the UI with the extracted data.
        /// </summary>
        /// <remarks>This method performs the file processing operation in a background thread and ensures
        /// that the UI is updated with the extracted data upon completion. If the <paramref name="uri"/> is null or not
        /// an absolute URI, the method exits without performing any operation.  The method handles exceptions
        /// internally to ensure that the application remains stable during the file processing.</remarks>
        /// <param name="uri">The URI of the file to process. Must be an absolute URI.</param>
        private async void ProcessInputFile(Uri uri)
        {
            if (uri == null || !uri.IsAbsoluteUri) return;

            IsLoadingFile = true;
            await ExceptionSafeAsync(async () =>
            {
                await backgroundExecutor.ExecuteAsync(() =>
                {
                    return ProcessFileAndUpdateUI(
                        uri,
                        () => OfficeParserPicker.GetOfficeParser(uri)
                            .GetExtractedFiles(uri)
                            .ToList(),
                        false);
                });
            },
            () =>
            {
                IsLoadingFile = false;
            });
        }

        /// <summary>
        /// Processes the specified file source, extracts embedded files, and returns an action to update the UI state.
        /// </summary>
        /// <remarks>The returned action updates the UI state by setting the input file, managing
        /// navigation state, and applying filters and sorting to the extracted files. If <paramref
        /// name="isNavigating"/> is <see langword="false"/>, the navigation stack is reset.</remarks>
        /// <typeparam name="T">The type of the file source, which must be either <see cref="Uri"/> or <see cref="string"/>.</typeparam>
        /// <param name="source">The file source to process. Must be either a <see cref="Uri"/> or a file path as a <see cref="string"/>.</param>
        /// <param name="getEmbeddedFiles">A function that retrieves a list of embedded files from the source.</param>
        /// <param name="isNavigating">A value indicating whether the operation is part of a navigation process.</param>
        /// <returns>An <see cref="Action"/> that updates the UI state based on the processed file and extracted files.</returns>
        /// <exception cref="ArgumentException">Thrown if <paramref name="source"/> is not of type <see cref="Uri"/> or <see cref="string"/>.</exception>
        private Action ProcessFileAndUpdateUI<T>(T source, Func<List<ExtractedFile>> getEmbeddedFiles, bool isNavigating)
            where T : class
        {
            IParseOffice parseOffice;
            InputFileViewModel inputFile;

            if (source is Uri uri)
            {
                parseOffice = OfficeParserPicker.GetOfficeParser(uri);
                inputFile = new InputFileViewModel(uri, parseOffice.OfficeType);
            }
            else if (source is string filePath)
            {
                parseOffice = OfficeParserPicker.GetOfficeParser(filePath);
                inputFile = new InputFileViewModel(new Uri($"file:///{filePath}"), parseOffice.OfficeType);
            }
            else
            {
                throw new ArgumentException($"Source must be either Uri or string, but was {source?.GetType().Name ?? "null"}");
            }

            List<ExtractedFile> embeddedFiles = getEmbeddedFiles();
            ExtractedFiles = CreateExtractedFileViewModels(embeddedFiles);

            return () =>
            {
                InputFile = inputFile;
                IsLoadingFile = false;

                if (!isNavigating)
                {
                    navigationStack = new Stack<NagivationState>();
                }

                NagivationState state = new NagivationState(ExtractedFiles, InputFile);
                navigationStack.Push(state);

                CanGoBack = navigationStack.Count > 1;
                currentNavigationState = state;

                ApplyFilter();
                SortExtractedFiles(FilteredExtractedFiles);
            };
        }

        /// <summary>
        /// Creates a collection of <see cref="ExtractedFileViewModel"/> instances based on the provided list of <see
        /// cref="ExtractedFile"/> objects.
        /// </summary>
        /// <remarks>Each <see cref="ExtractedFileViewModel"/> is initialized with dependencies and event
        /// handlers to update the UI when the <see cref="ExtractedFileViewModel.IsSelected"/> property changes. This
        /// ensures that related properties, such as <see cref="HasSelectedItems"/> and <see
        /// cref="SelectedFilesContainWarnings"/>, are updated accordingly.</remarks>
        /// <param name="embeddedFiles">A list of <see cref="ExtractedFile"/> objects to be converted into view models. Cannot be null.</param>
        /// <returns>A list of <see cref="ExtractedFileViewModel"/> instances representing the provided embedded files.</returns>
        private List<ExtractedFileViewModel> CreateExtractedFileViewModels(List<ExtractedFile> embeddedFiles)
        {
            List<ExtractedFileViewModel> result = new List<ExtractedFileViewModel>();
            foreach (ExtractedFile file in embeddedFiles)
            {
                ExtractedFileViewModel extractedFileVM = new ExtractedFileViewModel(WindowService, file, SaveFile, OpenFile, OpenFileInFox);

                // Update the UI when the IsSelected property changes
                extractedFileVM.PropertyChanged += (sender, e) =>
                {
                    if (e.PropertyName == nameof(ExtractedFileViewModel.IsSelected))
                    {
                        OnPropertyChanged(nameof(HasSelectedItems));
                        OnPropertyChanged(nameof(SelectedFilesContainWarnings));
                    }
                };

                result.Add(extractedFileVM);
            }
            return result;
        }


        private void PopFromNaviagationStack()
        {
            navigationStack.Pop();
            currentNavigationState = navigationStack.Peek();
            ExtractedFiles = currentNavigationState.ExtractedFiles ?? new List<ExtractedFileViewModel>();
            InputFile = currentNavigationState.InputFile ?? new InputFileViewModel();
            ApplyFilter();
            SortExtractedFiles(FilteredExtractedFiles);
            CanGoBack = navigationStack.Count > 1;
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