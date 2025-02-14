using System.IO;
using System.IO.Compression;

namespace FileObjectExtractor.Extensions
{
    public static class ZipArchiveExtryExtensions
    {
        public static byte[] GetBytes(this ZipArchiveEntry entry)
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
