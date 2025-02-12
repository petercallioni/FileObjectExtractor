using FileObjectExtractorTests;

namespace FileObjectExtractor.Models.Tests
{
    [TestClass()]
    public class FileReaderTests
    {
        private List<ExtractedFile> files;
        public FileReaderTests()
        {
            FileReader reader = new FileReader();
            files = reader.ParseFile(TestResources.DOCX);
        }

        [TestMethod()]
        public void ParseFileNameTest()
        {
            Assert.IsTrue(files.Select(x => x.FileName).Contains("EmbeddedTestDocx.docx"));
            Assert.IsTrue(files.Select(x => x.FileName).Contains("EmbeddedTestPDF.pdf"));
        }


        [TestMethod()]
        public void ParseFileExtractDocxTest()
        {
            string embeddedDocxSha256 = "6F003AA51A0ACB6B3CE172084BB8B17E75B60D3A2B5DE88AF758D36903ABB6A4";

            Assert.AreEqual(embeddedDocxSha256, GetHashSHA256(files
                .Where(x => x.FileName == "EmbeddedTestDocx.docx")
                .Select(x => x.EmbeddedFile)
                .First()));
        }

        [TestMethod()]
        public void ParseFileExtractPdfTest()
        {
            string embeddedPdfSha256 = "1FD733DF0DC966147734C6C858456A25E3B8D3855DEA0BDBBE6E6767F7E05107";
            FileController fileController = new FileController(null!);
            byte[] extractedFile = fileController.StripHeaderAndTrailer(files
                .Where(x => x.FileName == "EmbeddedTestPDF.pdf")
                .Select(x => x.EmbeddedFile)
                .First());

            Assert.AreEqual(embeddedPdfSha256, GetHashSHA256(extractedFile));
        }

        [TestMethod()]
        public void ParseFileExtractJsonTest()
        {
            string embeddedPdfSha256 = "97EA2C1DB6D41CED3A20E914C353A3F95121D9DCE6843AE2F3990CC834991B0C";
            FileController fileController = new FileController(null!);
            byte[] extractedFile = fileController.StripHeaderAndTrailer(files
                .Where(x => x.FileName == "EmbeddedJson.json")
                .Select(x => x.EmbeddedFile)
                .First());

            Assert.AreEqual(embeddedPdfSha256, GetHashSHA256(extractedFile));
        }

        private string GetHashSHA256(byte[] data)
        {
            using (System.Security.Cryptography.SHA256 sha256 = System.Security.Cryptography.SHA256.Create())
            {
                return string.Concat(sha256.ComputeHash(data).Select(x => x.ToString("X2")));
            }
        }


    }
}