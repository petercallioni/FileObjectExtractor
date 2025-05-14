using System;
using System.IO;
using System.Threading.Tasks;

namespace FileObjectExtractor.Extensions
{
    public static class FileExtensions
    {
        public static byte[] ReadAllBytesReadOnly(Uri uri)
        {
            byte[] fileBytes;
            using (FileStream stream = new FileStream(
                uri.UnescapedString(),
                FileMode.Open,
                FileAccess.Read,
                FileShare.ReadWrite))
            {
                fileBytes = new byte[stream.Length];
                stream.Read(fileBytes, 0, fileBytes.Length);
            }
            ;

            return fileBytes;
        }

        public static byte[] ReadAllBytesReadOnly(string path)
        {
            byte[] fileBytes;
            using (FileStream stream = new FileStream(
                path,
                FileMode.Open,
                FileAccess.Read,
                FileShare.ReadWrite))
            {
                fileBytes = new byte[stream.Length];
                stream.Read(fileBytes, 0, fileBytes.Length);
            }
            ;

            return fileBytes;
        }

        public async static Task<byte[]> ReadAllBytesReadOnlyAync(Uri uri)
        {

            byte[] fileBytes;
            using (FileStream stream = new FileStream(
                uri.UnescapedString(),
                FileMode.Open,
                FileAccess.Read,
                FileShare.ReadWrite,
                4096,
                true))
            {
                fileBytes = new byte[stream.Length];
                await stream.ReadAsync(fileBytes, 0, fileBytes.Length);
            }
            return fileBytes;
        }

        public async static Task<byte[]> ReadAllBytesReadOnlyAsync(string path)
        {
            byte[] fileBytes;
            using (FileStream stream = new FileStream(
                path,
                FileMode.Open,
                FileAccess.Read,
                FileShare.ReadWrite,
                4096,
                true))
            {
                fileBytes = new byte[stream.Length];
                await stream.ReadAsync(fileBytes, 0, fileBytes.Length);
            }
            return fileBytes;
        }
    }
}
