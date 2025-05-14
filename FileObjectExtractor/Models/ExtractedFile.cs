using FileObjectExtractor.Constants;
using FileObjectExtractor.Extensions;
using OpenMcdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace FileObjectExtractor.Models
{
    public class ExtractedFile
    {
        private readonly byte[] embeddedFile;
        private string fileName;
        private bool isBinary;
        private string safeFileName;
        private List<string> fileNameWarnings;
        private int documentOrder;
        private bool isLinkedFile;
        private bool hasFileContent;

        public string FileName
        {
            get => fileName; set
            {
                fileName = value;
                SetSafeFileNameAndWarnings();
            }
        }

        public byte[] EmbeddedFile => embeddedFile;
        private string embeddedExtension;

        public bool IsBinary { get => isBinary; set => isBinary = value; }
        public string SafeFileName { get => safeFileName; set => safeFileName = value; }
        public List<string> FileNameWarnings { get => fileNameWarnings; set => fileNameWarnings = value; }
        public int DocumentOrder { get => documentOrder; set => documentOrder = value; }
        public bool IsLinkedFile { get => isLinkedFile; set => isLinkedFile = value; }
        public bool HasFileContent { get => hasFileContent; set => hasFileContent = value; }

        public ExtractedFile(byte[] embeddedFile)
        {
            this.embeddedFile = embeddedFile;
            this.isBinary = false;
            fileNameWarnings = new List<string>();
            fileName = string.Empty;
            safeFileName = string.Empty;
            embeddedExtension = string.Empty;
            hasFileContent = true;
        }

        public ExtractedFile(ZipArchiveEntry archivedFileEntry)
        {
            isLinkedFile = false;
            embeddedExtension = Path.GetExtension(archivedFileEntry.Name);
            isBinary = embeddedExtension.Equals(".bin", StringComparison.OrdinalIgnoreCase);
            embeddedFile = isBinary ? ExtractEmbeddedData(archivedFileEntry.GetBytes(), out isLinkedFile) : archivedFileEntry.GetBytes();
            hasFileContent = embeddedFile.Length != 0;

            fileNameWarnings = new List<string>();
            fileName = string.Empty;
            safeFileName = string.Empty;
        }

        private void SetSafeFileNameAndWarnings()
        {
            StringBuilder safeFileNameBuilder = new StringBuilder(fileName);
            FileInfo fileInfo = new FileInfo(fileName);

            if (safeFileNameBuilder.Length > IntContstants.MAX_FILE_NAME_CHARS)
            {
                Regex regex = new Regex(@"\.\w+$"); // Extension

                if (!regex.IsMatch(safeFileNameBuilder.ToString()))
                {
                    safeFileNameBuilder.Remove(IntContstants.MAX_FILE_NAME_CHARS, safeFileNameBuilder.Length - IntContstants.MAX_FILE_NAME_CHARS);
                    safeFileNameBuilder.Append("...");
                    safeFileNameBuilder.Append(embeddedExtension);
                    fileNameWarnings.Add(StringConstants.WARNINGS.LONG_FILENAME);
                }
            }

            if (fileInfo.Extension.Equals(""))
            {
                if (!isBinary)
                {
                    safeFileNameBuilder.Append(embeddedExtension);
                }
                else
                {

                    // If no explicit name was provided, try guessing the extension using magic bytes.
                    if (MagicBytes.FileType.GuessFileType(embeddedFile, out string extension))
                    {
                        safeFileNameBuilder.Append(extension);
                        fileNameWarnings.Add(StringConstants.WARNINGS.GUESSED_EXTENSION);
                    }
                    else
                    {
                        fileNameWarnings.Add(StringConstants.WARNINGS.NO_EXTENSION);
                    }
                }
            }

            if (StripInvalidCharacters(safeFileNameBuilder))
            {
                fileNameWarnings.Add(StringConstants.WARNINGS.INVALID_CHARACTERS);
            }

            if (IsLinkedFile)
            {
                safeFileNameBuilder.Replace(" (Command Line)", "");
                fileNameWarnings.Add(StringConstants.WARNINGS.LINKED_FILE);
            }

            safeFileName = safeFileNameBuilder.ToString();
        }

        private bool StripInvalidCharacters(StringBuilder stringBuilder)
        {
            FilenameSanitiser filenameSanitiser = new FilenameSanitiser();
            return filenameSanitiser.SanitiseFilename(stringBuilder);
        }

        private byte[] ExtractEmbeddedData(byte[] inputBin, out bool isLinkedFile)
        {
            byte[] contentArray = [];
            bool isLinkedFileLocal = false;
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
                            contentArray = ExtractOle10NativeData(bytes, out isLinkedFileLocal);
                            return;
                        }
                    }
                }, false); // The second parameter indicates whether to visit sub-storages recursively                
            }

            isLinkedFile = isLinkedFileLocal;
            return contentArray;
        }

        private byte[] ExtractOle10NativeData(byte[] data, out bool isLinkedFile)
        {
            Queue<byte> bytes = new Queue<byte>(data);
            List<byte> fileName = new List<byte>();
            List<byte> sourcePath = new List<byte>();
            byte[] returnBytes;
            isLinkedFile = false;

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

                // Tries to filter out files linked to
                byte[] isLink = bytes.DequeueMultiple(4);
                if (isLink.SequenceEqual(new byte[] { 0x00, 0x00, 0x01, 0x00 }))
                {
                    isLinkedFile = true;

                    if (TryGetLinkedFile(Encoding.UTF8.GetString(sourcePath.ToArray()), out byte[] linkedFile))
                    {
                        return linkedFile;
                    }

                    return new byte[0];
                }

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

        public bool TryGetLinkedFile(string path, out byte[] linkedFile)
        {
            if (File.Exists(path))
            {
                try
                {
                    linkedFile = FileExtensions.ReadAllBytesReadOnly(path);
                    return true;
                }
                catch // Dont try that hard to get the file
                {
                    linkedFile = new byte[0];
                    return false;
                }
            }
            else
            {
                linkedFile = new byte[0];
                return false;
            }
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