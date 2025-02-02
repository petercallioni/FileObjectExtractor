using CommunityToolkit.Mvvm.Input;
using MicrosoftObjectExtractor.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace MicrosoftObjectExtractor.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBase
    {
        private ObservableCollection<ExtractedFileVM> extractedFiles;
        private FileReader fileReader { get; init; }
        private string droppedFile;

        public IRelayCommand ProcessCommand
        {
            get; init;
        }

        public ObservableCollection<ExtractedFileVM> ExtractedFiles { get => extractedFiles; set => extractedFiles = value; }
        public string DroppedFile { get => droppedFile; set => droppedFile = value; }

        public MainWindowViewModel()
        {
            extractedFiles = new ObservableCollection<ExtractedFileVM>();
            fileReader = new FileReader();
            droppedFile = string.Empty;
            ProcessCommand = new RelayCommand(ProcessSelectedItems);
        }

        private void ProcessSelectedItems()
        {
            ExtractedFiles.Clear();

            List<ExtractedFile> embeddedFiles = fileReader.ParseFile(DroppedFile);

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