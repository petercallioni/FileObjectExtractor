using CommunityToolkit.Mvvm.Input;
using FileObjectExtractor.Models;
using System;

namespace FileObjectExtractor.ViewModels
{
    public class ExtractedFileViewModel : ViewModelBase
    {
        private ExtractedFile extractedFile;
        private bool isSelected;
        private bool isVisible;
        private readonly IRelayCommand saveFileCommand;
        private readonly IRelayCommand openFileCommand;
        private readonly string originalSafeFileName;
        private bool isOpen;
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
        public IRelayCommand OpenFileCommand => openFileCommand;
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
        public bool IsVisible
        {
            get => isVisible; set
            {
                if (value != isVisible)
                {
                    isVisible = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool IsOpen
        {
            get => isOpen; set
            {
                if (value != isOpen)
                {
                    isOpen = value;
                    OnPropertyChanged();
                }
            }
        }

        private string resetToolTip;

        private void ResetName()
        {
            FileName = originalSafeFileName;
        }

        public ExtractedFileViewModel(ExtractedFile extractedFile, Action<ExtractedFileViewModel> saveFileOperation, Action<ExtractedFileViewModel> openFileOperation)
        {
            this.extractedFile = extractedFile;
            originalSafeFileName = extractedFile.SafeFileName;
            saveFileCommand = new RelayCommand(() => saveFileOperation(this));
            openFileCommand = new RelayCommand(() => openFileOperation(this));
            IsSelected = false;
            IsVisible = true;

            resetToolTip = $"Reset file name to {originalSafeFileName}.";
        }
    }
}
