using FileObjectExtractor.Models;
using FileObjectExtractor.Models.MagicBytes;
using FileObjectExtractor.Models.Office;
using FileObjectExtractorTests;

namespace FileObjectExtractor.Services.Tests
{
    [TestClass()]
    public class FileTypeMagicBytesTests
    {
        [TestMethod()]
        public void GuessFileTypeTest()
        {
            IParseOffice parseOffice = OfficeParserPicker.GetOfficeParser(TestResources.DOCX);
            List<ExtractedFile> files = parseOffice.GetExtractedFiles(TestResources.DOCX);

            FileController fileController = new FileController(null!);
            byte[] extractedPng = files
                .Where(x => x.FileName == "EmbeddedPng.png")
                .Select(x => x.EmbeddedFile)
                .First();

            byte[] extractedMp3 = files
                .Where(x => x.FileName == "EmbeddedMp3.mp3")
                .Select(x => x.EmbeddedFile)
                .First();

            FileType.GuessFileType(extractedPng, out string extension1);
            FileType.GuessFileType(extractedMp3, out string extension2);

            Assert.IsTrue(extension1.Equals(".png", StringComparison.OrdinalIgnoreCase), $"Expected .png, but got {extension1}");
            Assert.IsTrue(extension2.Equals(".mp3", StringComparison.OrdinalIgnoreCase), $"Expected .mp3, but got {extension2}");
        }
    }
}