using System.IO;

namespace FileObjectExtractor.Updates
{
    public record DownloadedUpdateFiles(FileInfo ChecksumFile, FileInfo ArchiveFile);
}