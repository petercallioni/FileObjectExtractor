using FileObjectExtractor.Models;

namespace FileObjectExtractor.ViewModels
{
    public class DesignMainViewModel : MainWindowViewModel
    {
        public DesignMainViewModel() : base(null!, null!)
        {
            // Add mock data here
            DroppedFile = null;

            for (int i = 0; i < 4; i++)
            {
                ExtractedFile extractedFile = new ExtractedFile(new byte[1000 * i]);
                ExtractedFileViewModel extractedFileVM = new ExtractedFileViewModel(extractedFile, null!);
                extractedFileVM.IsSelected = i % 2 == 0;

                if (i == 3)
                {
                    extractedFileVM.ExtractedFile.FileName = $"This is a very long filename that is over 50 characters long.docx";
                }
                else
                {
                    extractedFileVM.ExtractedFile.FileName = $"Embedded File {i}.docx";
                }

                ExtractedFiles.Add(extractedFileVM);
            }
        }
    }
}
