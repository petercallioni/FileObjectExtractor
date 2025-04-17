using FileObjectExtractor.Models.Office;
using FileObjectExtractorTests;

namespace FileObjectExtractor.Models.Tests
{
    [TestClass()]
    public class FileReaderDocxImageOnlyTests
    {
        private List<ExtractedFile> files;
        public FileReaderDocxImageOnlyTests()
        {
            IParseOffice parseOffice = OfficeParserPicker.GetOfficeParser(TestResources.DOCX_ONLY_IMAGE);
            files = parseOffice.GetExtractedFiles(TestResources.DOCX_ONLY_IMAGE);
        }

        [TestMethod()]
        public void ParseFileNameTest()
        {
            Assert.IsTrue(files.Select(x => x.FileName).Contains("image1.png"));
        }
    }
}