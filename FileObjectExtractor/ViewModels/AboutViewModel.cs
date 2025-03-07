using CommunityToolkit.Mvvm.Input;
using FileObjectExtractor.Services;
using System.Reflection;

namespace FileObjectExtractor.ViewModels
{
    public class AboutViewModel : ClosableViewModel
    {
        private string versionNumber;
        private string author;

        private string buildDate;
        public IRelayCommand CloseCommand => new RelayCommand(Close);

        public string VersionNumber { get => versionNumber; set => versionNumber = value; }
        public string Author { get => author; set => author = value; }
        public string BuildDate { get => buildDate; set => buildDate = value; }

        public AboutViewModel(IWindowService windowService) : base(windowService)
        {
            author = "Peter Callioni";
            buildDate = System.IO.File.GetLastWriteTime(Assembly.GetExecutingAssembly().Location).ToString("yyyy-MM-dd");
            versionNumber = Assembly.GetExecutingAssembly()?.GetName()?.Version?.ToString() ?? "Unknown";
        }
    }
}
