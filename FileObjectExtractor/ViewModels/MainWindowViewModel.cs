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

        public IRelayCommand ProcessCommand
        {
            get; init;
        }

        public ObservableCollection<ExtractedFileVM> ExtractedFiles { get => extractedFiles; set => extractedFiles = value; }
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

        public MainWindowViewModel()
        {
            extractedFiles = new ObservableCollection<ExtractedFileVM>();
            filePassword = string.Empty;
            fileReader = new FileReader();
            droppedFile = DefaultText;
            ProcessCommand = new RelayCommand(ProcessSelectedItems);
        }

        private void ProcessSelectedItems()
        {
            ExtractedFiles.Clear();

            List<ExtractedFile> embeddedFiles = fileReader.ParseFile(DroppedFile)
                .OrderBy(x => x.FileName)
                .ToList();

            foreach (ExtractedFile file in embeddedFiles)
            {
                ExtractedFileVM extractedFileVM = new ExtractedFileVM(file);
                ExtractedFiles.Add(extractedFileVM);
            }
        }

        public void SelectFile(string filePath)
        {
            DroppedFile = filePath;
            ProcessSelectedItems();
        }
    }
}