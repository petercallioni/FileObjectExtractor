using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace FileObjectExtractor.Utilities
{
    public static class TemporaryFiles
    {
        public static DirectoryInfo GetTemporaryDirectory()
        {
            string tempPath = Path.GetTempPath();
            string? tempFoxParentDir = Assembly.GetExecutingAssembly().GetName().Name;

            if (tempFoxParentDir != null)
            {
                tempPath = Path.Combine(tempPath, tempFoxParentDir);
            }

            DirectoryInfo tempDir = new DirectoryInfo(tempPath);
            if (!tempDir.Exists)
            {
                tempDir.Create();
            }

            return tempDir;
        }

        public static FileInfo CreateTemporaryFile(string fileName)
        {
            return new FileInfo(Path.Combine(GetTemporaryDirectory().FullName, fileName));
        }

        public static List<FileInfo> GetTemporaryFiles()
        {
            DirectoryInfo tempDir = GetTemporaryDirectory();
            return tempDir.EnumerateFiles().ToList();
        }

        public static long GetTemporaryFilesSize()
        {
            List<FileInfo> tempFiles = GetTemporaryFiles();

            return tempFiles.Select(file => file.Length).Sum();
        }

        public static string GetTemporaryFilesSizeHumanReadable()
        {
            long size = GetTemporaryFilesSize();
            return ByteSizeFormatter.Format(size);
        }

        /// <summary>
        /// Clears all temporary files in the temporary directory.
        /// Could throw an exception if a file can not be deleted.
        /// </summary>
        public static void ClearTemporaryFiles()
        {
            List<FileInfo> tempFiles = GetTemporaryFiles();
            foreach (FileInfo file in tempFiles)
            {
                file.Delete();
            }
        }
    }
}