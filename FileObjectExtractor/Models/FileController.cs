using Avalonia.Controls;
using Avalonia.Platform.Storage;
using System.IO;
using System.Threading.Tasks;

namespace FileObjectExtractor.Models
{
    public class FileController
    {
        private Window window;
        public FileController(Window window)
        {
            this.window = window;
        }

        public async Task<IStorageFile?> OpenFileAsync()
        {
            System.Collections.Generic.IReadOnlyList<IStorageFile> files = await window.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions()
            {
                Title = "Open File",
                AllowMultiple = false
            });

            return files.Count >= 1 ? files[0] : null;
        }

        public async Task<bool> SaveFileAsync(ExtractedFile extractedFile)
        {
            FilePickerSaveOptions saveOptions = new FilePickerSaveOptions();
            saveOptions.Title = "Save File";
            saveOptions.SuggestedFileName = extractedFile.FileName;
            IStorageFile? file = await window.StorageProvider.SaveFilePickerAsync(saveOptions);

            if (file != null)
            {
                File.WriteAllBytes(file.Path.AbsolutePath, extractedFile.EmbeddedFile);
                return true;
            }

            return false;
        }
    }
}