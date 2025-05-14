using CommunityToolkit.Mvvm.Input;
using FileObjectExtractor.Services;
using FileObjectExtractor.Utilities;

namespace FileObjectExtractor.ViewModels
{
    public class TemporaryFilesViewModel : ViewModelBase
    {
        private int fileCount;
        private string fileSize;

        private IRelayCommand clearFilesCommand;
        public int FileCount
        {
            get => fileCount;
            set
            {
                fileCount = value;
                OnPropertyChanged();
            }
        }
        public string FileSize
        {
            get => fileSize;
            set
            {
                fileSize = value;
                OnPropertyChanged();
            }
        }

        public IRelayCommand ClearFilesCommand { get => clearFilesCommand; set => clearFilesCommand = value; }

        public TemporaryFilesViewModel(IWindowService windowService) : base(windowService)
        {
            fileCount = 0;
            fileSize = string.Empty;
            clearFilesCommand = new RelayCommand(ClearTemporaryFiles);
            RefreshTemporaryFilesInfo();
        }

        public void RefreshTemporaryFilesInfo()
        {
            FileCount = TemporaryFiles.GetTemporaryFiles().Count;
            FileSize = TemporaryFiles.GetTemporaryFilesSizeHumanReadable();
        }

        public void ClearTemporaryFiles()
        {
            ExceptionSafe(() => TemporaryFiles.ClearTemporaryFiles());
            RefreshTemporaryFilesInfo();
        }
    }
}