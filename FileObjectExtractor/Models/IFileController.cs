using Avalonia.Platform.Storage;
using FileObjectExtractor.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FileObjectExtractor.Models
{
    public interface IFileController
    {
        Task<IStorageFile?> AskOpenFileAsync();
        Task<bool> AskSaveFileAsync(ExtractedFile extractedFile, IProgressService? progressService = null);
        Task<bool> AskSaveMultipleFiles(List<ExtractedFile> files, IProgressService? progressService = null);
        void OpenFile(ExtractedFile extractedFile);
        void SaveFile(string filePath, ExtractedFile file);
    }
}