using Avalonia.Platform.Storage;
using FileObjectExtractor.Extensions;
using FileObjectExtractor.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace FileObjectExtractor.Models
{
    public class FileController : IFileController
    {
        private IWindowService? window;

        public FileController(IWindowService? window)
        {
            this.window = window;
        }

        public async Task<IStorageFile?> AskOpenFileAsync()
        {
            if (window == null)
            {
                throw new InvalidOperationException("Called UI method when not using a UI.");
            }

            return await window.OpenFileAsync("Select a file");
        }

        public void OpenFile(ExtractedFile extractedFile)
        {
            string tempPath = Path.GetTempPath();
            string tempFile = Path.Combine(tempPath, extractedFile.SafeFileName);
            File.WriteAllBytes(tempFile, extractedFile.EmbeddedFile);

            Task.Run(() =>
            {
                try
                {
                    Process subProcess = new Process();
                    subProcess.StartInfo.UseShellExecute = true;
                    subProcess.StartInfo.FileName = tempFile;
                    subProcess.Start();
                    subProcess.WaitForExit();
                }
                finally
                {
                    File.Delete(tempFile);
                }
            });
        }

        public async Task<bool> AskSaveMultipleFiles(List<ExtractedFile> files, IProgressService? progressService = null)
        {
            if (window == null)
            {
                throw new InvalidOperationException("Called UI method when not using a UI.");
            }

            string? selectedFolderPath = await window.SelectFolderAsync("Select a folder");
            int counter = 0;

            if (selectedFolderPath != null)
            {
                progressService?.ShowProgress();
                progressService?.SetProgress(0);
                progressService?.SetMaximum(files.Count);

                foreach (ExtractedFile file in files)
                {
                    progressService?.SetMessage($"Writing {file.SafeFileName}");

                    await Task.Run(() =>
                    {
                        string filePath = Path.Combine(selectedFolderPath, file.SafeFileName);
                        filePath = GetUniqueFilePath(filePath);
                        SaveFile(filePath, file);

                    });

                    progressService?.SetProgress(++counter);
                }

                progressService?.SetMessage($"Finished");
            }

            return true;
        }

        public async Task<bool> AskSaveFileAsync(ExtractedFile extractedFile, IProgressService? progressService = null)
        {
            if (window == null)
            {
                throw new InvalidOperationException("Called UI method when not using a UI.");
            }

            IStorageFile? file = await window.SaveFileAsync("Save File", extractedFile.SafeFileName);

            if (file != null)
            {
                progressService?.ShowProgress();
                progressService?.SetProgress(0);
                progressService?.SetMaximum(1);
                progressService?.SetMessage($"Writing {extractedFile.SafeFileName}");

                await Task.Run(() =>
                {
                    SaveFile(file.Path.UnescapedString(), extractedFile);
                });

                progressService?.SetProgress(1);
                progressService?.SetMessage($"Done {extractedFile.SafeFileName}");

                return true;
            }

            return false;
        }

        public void SaveFile(string filePath, ExtractedFile file)
        {
            File.WriteAllBytes(filePath, file.EmbeddedFile);
        }

        private string GetUniqueFilePath(string filePath)
        {
            int counter = 1;
            string directory = Path.GetDirectoryName(filePath)!;
            string fileName = Path.GetFileNameWithoutExtension(filePath);
            string extension = Path.GetExtension(filePath);

            while (File.Exists(filePath))
            {
                filePath = Path.Combine(directory, $"{fileName} ({counter}){extension}");
                counter++;
            }

            return filePath;
        }
    }
}