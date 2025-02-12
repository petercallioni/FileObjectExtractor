using Avalonia.Controls;
using Avalonia.Platform.Storage;
using FileObjectExtractor.Models.Converters;
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
            System.Collections.Generic.IReadOnlyList<IStorageFile> files = await window.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions()
            {
                Title = "Open File",
                AllowMultiple = false
            });

            return files.FirstOrDefault();
        }

        public async Task<bool> SaveFileAsync(ExtractedFile extractedFile)
        {
            FilePickerSaveOptions saveOptions = new FilePickerSaveOptions();
            saveOptions.Title = "Save File";
            saveOptions.SuggestedFileName = extractedFile.FileName;
            IStorageFile? file = await window.StorageProvider.SaveFilePickerAsync(saveOptions);

            using (MemoryStream memoryStream = new MemoryStream(extractedFile.EmbeddedFile))
            {
                CompoundFile cf = new CompoundFile(memoryStream);
                // Iterate through all the streams in the root storage
                cf.RootStorage.VisitEntries((x) =>
                {
                    Console.WriteLine(x.Name);
                },
                false);

                if (file != null)
                {
                    File.WriteAllBytes(file.Path.AbsolutePath, extractedFile.EmbeddedFile);
                    return true;
                }

                return false;
            }
        }

        public byte[] StripHeaderAndTrailer(byte[] inputBin)
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
            string text = Encoding.UTF8.GetString(data);
            List<byte> fileName = new List<byte>();
            List<byte> sourcePath = new List<byte>();

            bytes.DequeueMultiple<byte>(6);
            do
            {
                fileName.Add(bytes.Dequeue());
            } while (bytes.First() != 0);

            bytes.Dequeue();

            do
            {
                sourcePath.Add(bytes.Dequeue());
            } while (bytes.First() != 0);

            bytes.Dequeue();

            bytes.DequeueMultiple(4);

            byte[] dwSize = bytes.DequeueMultiple(4);

            uint dwSizeInt = HexConverter.LittleEndianHexToUInt(Convert.ToHexString(dwSize));

            bytes.DequeueMultiple((int)dwSizeInt);

            byte[] dataSize = bytes.DequeueMultiple(4);
            uint dataSizeInt = HexConverter.LittleEndianHexToUInt(Convert.ToHexString(dataSize));
            byte[] returnData = bytes.DequeueMultiple((int)dataSizeInt);

            return returnData;
        }
    }
}