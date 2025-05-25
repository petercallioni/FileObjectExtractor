using FileObjectExtractorTests.Updates;

namespace FileObjectExtractor.Updates.Tests
{
    [TestClass()]
    public class UpdateServiceTests
    {

        [TestMethod()]
        public void VerifyChecksumTest()
        {
            IUpdateService mockService = new MockUpdateService();
            DownloadedUpdateFiles downloadedFiles = mockService.DownloadUpdate(null!).Result;
            UpdateService service = new UpdateService();

            Assert.IsTrue(service.VerifyChecksum(
                File.ReadAllBytes(downloadedFiles.ChecksumFile.FullName),
                File.ReadAllBytes(downloadedFiles.ArchiveFile.FullName),
                UpdateAssetFiles.WINDOWS_ARCHIVE_NAME,
                out string _));
        }
    }
}