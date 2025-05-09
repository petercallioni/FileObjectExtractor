using CommunityToolkit.Mvvm.Input;
using FileObjectExtractor.Services;

namespace FileObjectExtractor.ViewModels
{
    public class AboutViewModel : ClosableViewModel
    {
        private string versionNumber;
        private string author;
        public IRelayCommand CloseCommand => new RelayCommand(Close);

        public string VersionNumber { get => versionNumber; set => versionNumber = value; }
        public string Author { get => author; set => author = value; }

        public AboutViewModel(IWindowService windowService) : base(windowService)
        {
            author = "Peter Callioni";
            versionNumber = Models.VersionNumber.GetVersion();
        }
    }
}
