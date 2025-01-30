using MicrosoftObjectExtractorTests;

namespace MicrosoftObjectExtractor.Models.Tests
{
    [TestClass()]
    public class FileReaderTests
    {
        [TestMethod()]
        public void ParseFileTest()
        {
            FileReader reader = new FileReader();
            List<ExtractedFile> files = reader.ParseFile(TestResources.DOCX);

            Assert.IsTrue(files.Select(x => x.FileName).Contains("EmbeddedTestDocx.docx"));
            Assert.IsTrue(files.Select(x => x.FileName).Contains("EmbeddedTestPDF.pdf"));
        }
    }
}