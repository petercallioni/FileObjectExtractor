using FileObjectExtractor.Extensions;
using System.IO;
using System.IO.Compression;

namespace FileObjectExtractor.Models
{
    public class ExtractedFile
    {
        //private readonly byte[] iconFile;
        private readonly byte[] embeddedFile;
        private string fileName;
        private bool isBinary;

        public string FileName { get => fileName; set => fileName = value; }
        public byte[] EmbeddedFile => embeddedFile;
        //public byte[] IconFile => iconFile;

        public bool IsBinary { get => isBinary; set => isBinary = value; }

        public ExtractedFile(byte[] embeddedFile)
        {
            //this.iconFile = iconFile;
            this.embeddedFile = embeddedFile;
            this.isBinary = false;
            fileName = string.Empty;
        }

        public ExtractedFile(ZipArchiveEntry archivedFileEntry)
        {
            //iconFile = ExtractToMemory(iconFileEntry);
            embeddedFile = archivedFileEntry.GetBytes();
            isBinary = archivedFileEntry.Name.EndsWith(".bin", System.StringComparison.OrdinalIgnoreCase);
            fileName = string.Empty;
        }

        public void SaveEmbeddedFile(string path)
        {
            File.WriteAllBytes(path, EmbeddedFile);
        }
    }
}