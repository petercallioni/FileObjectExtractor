using FileObjectExtractor.Constants;
using FileObjectExtractor.Extensions;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
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

        public string FileName
        {
            get => fileName; set
            {
                fileName = value;
                SetSafeFileName();
            }
        }

        public byte[] EmbeddedFile => embeddedFile;

        public bool IsBinary { get => isBinary; set => isBinary = value; }
        public string SafeFileName { get => safeFileName; set => safeFileName = value; }
        public List<string> FileNameWarnings { get => fileNameWarnings; set => fileNameWarnings = value; }
        public int DocumentOrder { get => documentOrder; set => documentOrder = value; }

        public ExtractedFile(byte[] embeddedFile)
        {
            this.embeddedFile = embeddedFile;
            this.isBinary = false;
            fileNameWarnings = new List<string>();
            fileName = string.Empty;
            safeFileName = string.Empty;
        }

        public ExtractedFile(ZipArchiveEntry archivedFileEntry)
        {
            embeddedFile = archivedFileEntry.GetBytes();
            isBinary = archivedFileEntry.Name.EndsWith(".bin", System.StringComparison.OrdinalIgnoreCase);
            fileNameWarnings = new List<string>();
            fileName = string.Empty;
            safeFileName = string.Empty;
        }

        private void SetSafeFileName()
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
                    fileNameWarnings.Add(StringConstants.WARNINGS.LONG_FILENAME);
                }
            }

            if (fileInfo.Extension.Equals(""))
            {
                fileNameWarnings.Add(StringConstants.WARNINGS.NO_EXTENSION);
            }

            if (StripInvalidCharacters(safeFileNameBuilder))
            {
                fileNameWarnings.Add(StringConstants.WARNINGS.INVALID_CHARACTERS);
            }

            safeFileName = safeFileNameBuilder.ToString();
        }

        private bool StripInvalidCharacters(StringBuilder stringBuilder)
        {
            FilenameSanitiser filenameSanitiser = new FilenameSanitiser();
            return filenameSanitiser.SanitiseFilename(stringBuilder);
        }
    }
}