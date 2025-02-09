using FileObjectExtractor.Models;

namespace FileObjectExtractor.ViewModels
{
    public class DesignMainViewModel : MainWindowViewModel
    {
        public DesignMainViewModel()
        {
            // Add mock data here
            DroppedFile = MainWindowViewModel.DefaultText;
            FilePassword = "Password";

            for (int i = 0; i < 3; i++)
            {
                ExtractedFile extractedFile = new ExtractedFile(new byte[1], new byte[1000 * i]);
                ExtractedFileVM extractedFileVM = new ExtractedFileVM(extractedFile);
                extractedFileVM.ExtractedFile.FileName = $"Embedded File {i}.docx";
                ExtractedFiles.Add(extractedFileVM);
            }
        }
    }
}
