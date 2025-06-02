
using FileObjectExtractor.Updates;

namespace FileObjectExtractorTests.Updates
{
    /// <summary>
    /// These use mock data to simulate the behavior of the UpdateService.
    /// You will have to create the files used yourself
    /// </summary>
    public class MockUpdateService : IUpdateService
    {
        public Task<Update> CheckForUpdate()
        {
            return Task.Run(() =>
            {
                return new Update(
                "https://github.com/petercallioni/FileObjectExtractor/releases/download/v1.1.2/FileObjectExtractor-win-x64.zip",
                "https://github.com/petercallioni/FileObjectExtractor/releases/download/v1.1.2/checksums.txts",
                "v1.1.2",
                UpdateOS.WINDOWS,
                76546048L,
                DateTime.MinValue
                );
            });
        }

        public Task<DownloadedUpdateFiles> DownloadUpdate(Update update)
        {
            return Task.Run(() =>
            {
                // Return the paths of the downloaded files
                return new DownloadedUpdateFiles(
                    new FileInfo(Path.Combine(TestResources.RESOURCES_DIRECTORY.AbsolutePath, "Updates", UpdateAssetFiles.CHECKSUMS)),
                   new FileInfo(Path.Combine(TestResources.RESOURCES_DIRECTORY.AbsolutePath, "Updates", UpdateAssetFiles.WINDOWS_ARCHIVE_NAME)));
            });
        }

        public Task<DownloadedUpdateFiles> DownloadUpdate(Update update, IProgress<double>? progress = null!)
        {
            throw new NotImplementedException();
        }

        public Task<DownloadedUpdateFiles> DownloadUpdate(Update update, IProgress<DownloadProgressReport>? progress = null!)
        {
            throw new NotImplementedException();
        }

        public Task InstallUpdate(Update update, DownloadedUpdateFiles files)
        {
            throw new NotImplementedException();
        }

        public Task InstallUpdate(Update update, DownloadedUpdateFiles files, bool noRestart = false)
        {
            throw new NotImplementedException();
        }
    }
}
