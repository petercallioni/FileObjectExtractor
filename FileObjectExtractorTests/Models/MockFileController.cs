using Avalonia.Platform.Storage;
using FileObjectExtractor.Models;
using FileObjectExtractor.Services;

namespace FileObjectExtractorTests.Models
{
    public class MockFileController : IFileController
    {
        FileController fileController;

        public MockFileController()
        {
            fileController = new FileController(null!);
        }

        public Task<IStorageFile?> AskOpenFileAsync()
        {
            throw new NotImplementedException();
        }

        public Task<bool> AskSaveFileAsync(ExtractedFile extractedFile, IProgressService? progressService = null)
        {
            throw new NotImplementedException();
        }

        public Task<bool> AskSaveMultipleFiles(List<ExtractedFile> files, IProgressService? progressService = null)
        {
            throw new NotImplementedException();
        }

        public void OpenFile(ExtractedFile extractedFile)
        {
            throw new NotImplementedException();
        }

        public void SaveFile(string filePath, ExtractedFile file)
        {
            // Do Nothing
        }
    }
}
