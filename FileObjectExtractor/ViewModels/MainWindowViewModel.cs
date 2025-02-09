using CommunityToolkit.Mvvm.Input;
using FileObjectExtractor.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace FileObjectExtractor.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBase
    {
        protected static string DefaultText = "Drop\n+\nHere";
        private ObservableCollection<ExtractedFileVM> extractedFiles;
        private FileReader fileReader { get; init; }
        private string droppedFile;
        private string filePassword;
        private string filter;

        public IRelayCommand ProcessCommand { get; init; }
        public IRelayCommand SelectAllCommand { get; init; }
        public IRelayCommand SelectNoneCommand { get; init; }
        public IRelayCommand SaveSelectedCommand { get; init; }
        public IRelayCommand SelectFileCommand { get; init; }
        public ObservableCollection<ExtractedFileVM> ExtractedFiles { get => extractedFiles; set => extractedFiles = value; }

        private FileController fileController;
        public string DroppedFile
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

        public string FilePassword
        {
            get => filePassword; set
            {
                if (filePassword != value)
                {
                    {
                        filePassword = value;
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

        public MainWindowViewModel(FileController fileController)
        {
            extractedFiles = new ObservableCollection<ExtractedFileVM>();
            filePassword = string.Empty;
            filter = string.Empty;
            fileReader = new FileReader();
            droppedFile = DefaultText;
            ProcessCommand = new RelayCommand(ProcessSelectedItems);
            SelectAllCommand = new RelayCommand(SelectAll);
            SelectNoneCommand = new RelayCommand(SelectNone);
            SelectFileCommand = new RelayCommand(SelectFile);
            this.fileController = fileController;
        }

        private void ProcessSelectedItems()
        {
            ExtractedFiles.Clear();

            List<ExtractedFile> embeddedFiles = fileReader.ParseFile(DroppedFile)
                .OrderBy(x => x.FileName)
                .ToList();

            foreach (ExtractedFile file in embeddedFiles)
            {
                ExtractedFileVM extractedFileVM = new ExtractedFileVM(file, SaveFile);
                ExtractedFiles.Add(extractedFileVM);
            }

            ApplyFilter();
        }

        public void SelectFile(string filePath)
        {
            DroppedFile = filePath;
            ProcessSelectedItems();
        }

        public void SelectAll()
        {
            foreach (ExtractedFileVM item in ExtractedFiles)
            {
                item.IsSelected = true;
            }
        }

        public void SelectNone()
        {
            foreach (ExtractedFileVM item in ExtractedFiles)
            {
                item.IsSelected = false;
            }
        }

        private void ApplyFilter()
        {
            foreach (ExtractedFileVM item in ExtractedFiles)
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
        }

        private async void SelectFile()
        {
            Avalonia.Platform.Storage.IStorageFile? file = await fileController.OpenFileAsync();

            if (file != null)
            {
                DroppedFile = file.Path.ToString();
                ProcessSelectedItems();
            }
        }

        private async void SaveFile(ExtractedFileVM extractedFileVM)
        {
            bool saveSuccess = await fileController.SaveFileAsync(extractedFileVM.ExtractedFile);
        }
    }
}