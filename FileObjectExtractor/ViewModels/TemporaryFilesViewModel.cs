using CommunityToolkit.Mvvm.Input;
using FileObjectExtractor.Services;

namespace FileObjectExtractor.ViewModels
{
    public class TemporaryFilesViewModel : ViewModelBase
    {
        private int fileCount;
        private string fileSize;
        private string tempDirectory;

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
        public string TempDirectory { get => tempDirectory; set => tempDirectory = value; }

        public TemporaryFilesViewModel(IWindowService windowService) : base(windowService)
        {
            fileCount = 0;
            fileSize = string.Empty;
            clearFilesCommand = new RelayCommand(ClearTemporaryFiles);
            tempDirectory = Utilities.TemporaryFiles.GetTemporaryDirectory().FullName;
            RefreshTemporaryFilesInfo();
        }

        public void RefreshTemporaryFilesInfo()
        {
            FileCount = Utilities.TemporaryFiles.GetTemporaryFiles().Count;
            FileSize = Utilities.TemporaryFiles.GetTemporaryFilesSizeHumanReadable();
        }

        public void ClearTemporaryFiles()
        {
            ExceptionSafe(() => Utilities.TemporaryFiles.ClearTemporaryFiles());
            RefreshTemporaryFilesInfo();
        }
    }
}