using FileObjectExtractor.Models;

namespace FileObjectExtractor.ViewModels
{
    public class DesignMainViewModel : MainWindowViewModel
    {
        public DesignMainViewModel() : base(null!)
        {
            // Add mock data here
            DroppedFile = null;

            for (int i = 0; i < 3; i++)
            {
                ExtractedFile extractedFile = new ExtractedFile(new byte[1000 * i]);
                ExtractedFileVM extractedFileVM = new ExtractedFileVM(extractedFile, null!);
                extractedFileVM.IsSelected = i % 2 == 0;
                extractedFileVM.ExtractedFile.FileName = $"Embedded File {i}.docx";
                ExtractedFiles.Add(extractedFileVM);
            }
        }
    }
}
