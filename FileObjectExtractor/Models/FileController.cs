using Avalonia.Platform.Storage;
using FileObjectExtractor.Extensions;
using FileObjectExtractor.Services;
using OpenMcdf;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
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
            byte[] dataToSave = extractedFile.IsBinary ? ExtractEmbeddedData(extractedFile.EmbeddedFile) : extractedFile.EmbeddedFile;
            File.WriteAllBytes(tempFile, dataToSave);

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
            byte[] dataToSave = file.IsBinary ? ExtractEmbeddedData(file.EmbeddedFile) : file.EmbeddedFile;
            File.WriteAllBytes(filePath, dataToSave);
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

        public byte[] ExtractEmbeddedData(byte[] inputBin)
        {
            byte[] contentArray = [];
            using (MemoryStream memoryStream = new MemoryStream(inputBin))
            {
                CompoundFile cf = new CompoundFile(memoryStream);

                // Iterate through all the entries in the root storage
                cf.RootStorage.VisitEntries(entry =>
                {
                    // Check if the entry is a stream
                    if (entry.IsStream)
                    {
                        // Cast the entry to CFStream
                        CFStream stream = (CFStream)entry;

                        // Get the stream name
                        string streamName = stream.Name;
                        string text = Encoding.UTF8.GetString(stream.GetData());
                        byte[] bytes = stream.GetData();

                        if (streamName.Equals("CONTENTS"))
                        {
                            // Extract the binary data
                            contentArray = bytes;
                            return;
                        }
                        else if (streamName.Equals("\u0001Ole10Native"))
                        {
                            // Parse the Ole10Native data to extract the original data
                            contentArray = ExtractOle10NativeData(bytes);
                            return;
                        }
                    }
                }, false); // The second parameter indicates whether to visit sub-storages recursively                
            }

            return contentArray;
        }

        private byte[] ExtractOle10NativeData(byte[] data)
        {
            Queue<byte> bytes = new Queue<byte>(data);
            List<byte> fileName = new List<byte>();
            List<byte> sourcePath = new List<byte>();
            byte[] returnBytes;

            // Total Size, not including these 4 bytes
            bytes.DequeueMultiple(4);

            byte[] fileTypeIndicator = bytes.DequeueMultiple(2);

            if (fileTypeIndicator.SequenceEqual(new byte[] { 0x42, 0x4d })) // Indicates file type; Is a bit map
            {
                List<byte> bitMapBytes = new List<byte>(fileTypeIndicator);
                bitMapBytes.AddRange(bytes);
                TrimEndButOneNull(bitMapBytes);
                returnBytes = bitMapBytes.ToArray();
            }
            else if (fileTypeIndicator.SequenceEqual(new byte[] { 0x02, 0x00 })) // Indicates file type; Is normal binary file
            {
                // Get the file name, ends with null terminator
                byte currentByte;
                while ((currentByte = bytes.Dequeue()) != 0)
                {
                    fileName.Add(currentByte);
                }

                // Get the source path, ends with null terminator
                while ((currentByte = bytes.Dequeue()) != 0)
                {
                    sourcePath.Add(currentByte);
                }

                // Skip 4 bytes
                bytes.DequeueMultiple(4);

                // Gets the temporary file path size
                int dwSizeInt = BitConverter.ToInt32(bytes.DequeueMultiple(4), 0);
                bytes.DequeueMultiple(dwSizeInt); // Skip that many characters because we do not need them

                // Gets the actual data size
                int dataSizeInt = BitConverter.ToInt32(bytes.DequeueMultiple(4), 0);

                returnBytes = bytes.DequeueMultiple(dataSizeInt);
            }
            else
            {
                throw new NotImplementedException("This type of embedded file is unknown: " + Convert.ToHexString(fileTypeIndicator));
            }

            return returnBytes;
        }

        public void TrimEndButOneNull(List<byte> byteList)
        {
            if (byteList == null || byteList.Count == 0)
            {
                return;
            }

            int i = byteList.Count - 1;

            // Find the first non-null byte from the end
            while (i >= 0 && byteList[i] == 0)
            {
                i--;
            }

            // Remove all but one null byte
            byteList.RemoveRange(i + 2, byteList.Count - (i + 2));
        }
    }
}