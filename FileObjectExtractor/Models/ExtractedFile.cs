using System.IO;
using System.IO.Compression;

namespace FileObjectExtractor.Models
{
    public class ExtractedFile
    {
        private readonly byte[] iconFile;
        private readonly byte[] embeddedFile;
        private string? fileName;

        public string? FileName { get => fileName; set => fileName = value; }
        public byte[] EmbeddedFile => embeddedFile;
        public byte[] IconFile => iconFile;

        public ExtractedFile(ZipArchiveEntry iconFileEntry, ZipArchiveEntry archivedFileEntry)
        {
            iconFile = ExtractToMemory(iconFileEntry);
            embeddedFile = ExtractToMemory(archivedFileEntry);
        }

        private byte[] ExtractToMemory(ZipArchiveEntry entry)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (Stream entryStream = entry.Open())
                {
                    entryStream.CopyTo(memoryStream);
                }
                return memoryStream.ToArray();
            }
        }

        public void SaveEmbeddedFile(string path)
        {
            File.WriteAllBytes(path, EmbeddedFile);
        }
    }
}