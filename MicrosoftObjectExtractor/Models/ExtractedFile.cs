using System.IO;
using System.IO.Compression;

namespace MicrosoftObjectExtractor.Models
{
    public class ExtractedFile
    {
        public readonly byte[] IconFile;
        public readonly byte[] ArchivedFile;
        public string FileName;

        public ExtractedFile(ZipArchiveEntry iconFileEntry, ZipArchiveEntry archivedFileEntry)
        {
            IconFile = ExtractToMemory(iconFileEntry);
            ArchivedFile = ExtractToMemory(archivedFileEntry);
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
    }
}