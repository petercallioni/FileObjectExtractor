using CommunityToolkit.Mvvm.Input;
using FileObjectExtractor.Models;
using System;

namespace FileObjectExtractor.ViewModels
{
    public class ExtractedFileVM : ViewModelBase
    {
        private ExtractedFile extractedFile;
        private bool isSelected;
        private readonly IRelayCommand saveFileCommand;

        public IRelayCommand SaveFileCommand => saveFileCommand;
        public ExtractedFile ExtractedFile { get => extractedFile; set => extractedFile = value; }
        public bool IsSelected
        {
            get => isSelected; set
            {
                if (value != isSelected)
                {
                    isSelected = value;
                    OnPropertyChanged();
                }
            }
        }

        public ExtractedFileVM(ExtractedFile extractedFile, Action<ExtractedFileVM> saveFileOperation)
        {
            this.extractedFile = extractedFile;
            saveFileCommand = new RelayCommand(() => saveFileOperation(this));
            IsSelected = false;
        }
    }
}
