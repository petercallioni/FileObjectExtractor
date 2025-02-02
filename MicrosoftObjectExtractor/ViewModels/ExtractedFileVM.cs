using MicrosoftObjectExtractor.Models;

namespace MicrosoftObjectExtractor.ViewModels
{
    public class ExtractedFileVM : ViewModelBase
    {
        private ExtractedFile extractedFile;

        public ExtractedFile ExtractedFile { get => extractedFile; set => extractedFile = value; }
        public bool IsSelected { get; set; }

        public ExtractedFileVM(ExtractedFile extractedFile)
        {
            this.extractedFile = extractedFile;
            IsSelected = false;
        }
    }
}
