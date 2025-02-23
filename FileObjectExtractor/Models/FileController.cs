using Avalonia.Controls;
using Avalonia.Platform.Storage;
using FileObjectExtractor.Extensions;
using OpenMcdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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
            IReadOnlyList<IStorageFile> files = await window.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions()
            {
                Title = "Open File",
                AllowMultiple = false
            });

            return files.FirstOrDefault();
        }

        public async Task<bool> SaveMultipleFiles(List<ExtractedFile> files)
        {
            FolderPickerOpenOptions folderPickerOptions = new FolderPickerOpenOptions
            {
                Title = "Select Folder to Save Files"
            };
            IReadOnlyList<IStorageFolder> selectedFolder = await window.StorageProvider.OpenFolderPickerAsync(folderPickerOptions);
            string? selectedFolderPath = selectedFolder.FirstOrDefault()?.Path.AbsolutePath;

            if (selectedFolderPath != null)
            {
                foreach (ExtractedFile file in files)
                {
                    string filePath = Path.Combine(selectedFolderPath, file.FileName);
                    filePath = GetUniqueFilePath(filePath);
                    await File.WriteAllBytesAsync(filePath, file.EmbeddedFile);
                }
            }

            return true;
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

        public async Task<bool> SaveFileAsync(ExtractedFile extractedFile)
        {
            FilePickerSaveOptions saveOptions = new FilePickerSaveOptions();
            saveOptions.Title = "Save File";
            saveOptions.SuggestedFileName = extractedFile.FileName;
            IStorageFile? file = await window.StorageProvider.SaveFilePickerAsync(saveOptions);

            byte[] dataToSave = extractedFile.IsBinary ? ExtractEmbeddedData(extractedFile.EmbeddedFile) : extractedFile.EmbeddedFile;

            if (file != null)
            {
                File.WriteAllBytes(file.Path.AbsolutePath, dataToSave);
                return true;
            }

            return false;
        }

        public byte[] ExtractEmbeddedData(byte[] inputBin)
        {
            byte[] contentArray = [];
            using (MemoryStream memoryStream = new MemoryStream(inputBin))
            {
                CompoundFile cf = new CompoundFile(memoryStream);
                // Iterate through all the streams in the root storage

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

            if (fileTypeIndicator.SequenceEqual(new byte[] { 0x42, 0x4d })) // Indicates file type? Is a bit map
            {
                List<byte> bitMapBytes = new List<byte>(fileTypeIndicator);
                bitMapBytes.AddRange(bytes);
                TrimEndButOneNull(bitMapBytes);
                returnBytes = bitMapBytes.ToArray();
            }
            else if (fileTypeIndicator.SequenceEqual(new byte[] { 0x02, 0x00 })) // Indicates file type? Is normal binary file?
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