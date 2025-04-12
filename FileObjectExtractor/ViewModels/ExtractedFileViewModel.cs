using CommunityToolkit.Mvvm.Input;
using FileObjectExtractor.Models;
using System;

namespace FileObjectExtractor.ViewModels
{
    public class ExtractedFileViewModel : ViewModelBase
    {
        private ExtractedFile extractedFile;
        private bool isSelected;
        private readonly IRelayCommand saveFileCommand;
        private readonly string originalSafeFileName;
        public IRelayCommand ResetCommand => new RelayCommand(ResetName);

        public string FileName
        {
            get => extractedFile.SafeFileName;
            set
            {
                extractedFile.SafeFileName = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(NameChanged));
            }
        }

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

        public bool NameChanged => !string.Equals(originalSafeFileName, extractedFile.SafeFileName, StringComparison.OrdinalIgnoreCase);

        public string ResetToolTip { get => resetToolTip; set => resetToolTip = value; }

        private string resetToolTip;

        private void ResetName()
        {
            FileName = originalSafeFileName;
        }

        public ExtractedFileViewModel(ExtractedFile extractedFile, Action<ExtractedFileViewModel> saveFileOperation)
        {
            this.extractedFile = extractedFile;
            originalSafeFileName = extractedFile.SafeFileName;
            saveFileCommand = new RelayCommand(() => saveFileOperation(this));
            IsSelected = false;

            resetToolTip = $"Reset file name to {originalSafeFileName}.";
        }
    }
}
