﻿using CommunityToolkit.Mvvm.Input;
using FileObjectExtractor.Models;
using FileObjectExtractor.Services;
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
        private readonly IRelayCommand openFileInFoxCommand;
        private readonly string originalSafeFileName;
        private bool canOpen;
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
        public IRelayCommand OpenFileInFoxCommand => openFileInFoxCommand;
        public ExtractedFile ExtractedFile { get => extractedFile; set => extractedFile = value; }
        public bool IsSelected
        {
            get => isSelected; set
            {
                if (value != isSelected && extractedFile.HasFileContent)
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

        public bool CanOpen
        {
            get
            {
                if (!extractedFile.HasFileContent)
                {
                    return false;
                }

                return canOpen;
            }
            set
            {
                if (value != canOpen)
                {
                    canOpen = value;
                    OnPropertyChanged();
                }
            }
        }

        private string resetToolTip;

        private void ResetName()
        {
            FileName = originalSafeFileName;
        }

        public ExtractedFileViewModel(
            IWindowService windowService,
            ExtractedFile extractedFile,
            Action<ExtractedFileViewModel> saveFileOperation,
            Action<ExtractedFileViewModel> openFileOperation,
            Action<ExtractedFileViewModel> openFileInFoxOperation
            ) : base(windowService)
        {
            this.extractedFile = extractedFile;
            originalSafeFileName = extractedFile.SafeFileName;
            saveFileCommand = new RelayCommand(() => saveFileOperation(this));
            openFileCommand = new RelayCommand(() => openFileOperation(this));
            openFileInFoxCommand = new RelayCommand(() => openFileInFoxOperation(this));
            IsSelected = false;
            IsVisible = true;
            CanOpen = true;

            resetToolTip = $"Reset file name to {originalSafeFileName}.";
        }
    }
}
