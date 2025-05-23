﻿using FileObjectExtractor.Constants;
using FileObjectExtractor.Models.Office;
using FileObjectExtractorTests;

namespace FileObjectExtractor.Models.Tests
{
    [TestClass()]
    public class FileReaderPowerpointTests
    {
        private List<ExtractedFile> files;
        public FileReaderPowerpointTests()
        {
            IParseOffice parseOffice = OfficeParserPicker.GetOfficeParser(TestResources.POWERPOINT);
            files = parseOffice.GetExtractedFiles(TestResources.POWERPOINT);
        }

        [TestMethod()]
        public void ParseFileNameTest()
        {
            Assert.IsTrue(files.Select(x => x.FileName).Contains("Microsoft Word Document"));
        }

        [TestMethod()]
        public void ParseWordExtractDocxTest()
        {
            string embeddedDocxSha256 = "6F003AA51A0ACB6B3CE172084BB8B17E75B60D3A2B5DE88AF758D36903ABB6A4";

            Assert.AreEqual(embeddedDocxSha256, GetHashSHA256(files
                .Where(x => x.FileName == "Microsoft Word Document")
                .Select(x => x.EmbeddedFile)
                .First()));
        }

        [TestMethod()]
        public void ParseWordExtractPdfTest()
        {
            string embeddedPdfSha256 = "1FD733DF0DC966147734C6C858456A25E3B8D3855DEA0BDBBE6E6767F7E05107";
            FileController fileController = new FileController(null!);
            byte[] extractedFile = files
                .Where(x => x.FileName == "Adobe Acrobat Document")
                .Select(x => x.EmbeddedFile)
                .First();

            Assert.AreEqual(embeddedPdfSha256, GetHashSHA256(extractedFile));
        }

        [TestMethod()]
        public void ParseWordExtractJsonTest()
        {
            string embeddedPdfSha256 = "97EA2C1DB6D41CED3A20E914C353A3F95121D9DCE6843AE2F3990CC834991B0C";
            FileController fileController = new FileController(null!);
            byte[] extractedFile = files
                .Where(x => x.FileName == "EmbeddedJson.json")
                .Select(x => x.EmbeddedFile)
                .First();

            Assert.AreEqual(embeddedPdfSha256, GetHashSHA256(extractedFile));
        }

        [TestMethod()]
        public void ParseWordExtractPngTest()
        {
            string embeddedPdfSha256 = "4C9C00017F30C5EC004D247D80C2B2825ABA68510A7B8520EC9692B2E8143ECB";
            FileController fileController = new FileController(null!);
            byte[] extractedFile = files
                .Where(x => x.FileName == "EmbeddedPng.png")
                .Select(x => x.EmbeddedFile)
                .First();

            Assert.AreEqual(embeddedPdfSha256, GetHashSHA256(extractedFile));
        }

        [TestMethod()]
        public void ParseWordExtractMp3Test()
        {
            string embeddedPdfSha256 = "3593C7D216AF0EA386577FC96703E6884010AB590A7897CDFED39530BA15DE0B";
            FileController fileController = new FileController(null!);
            byte[] extractedFile = files
                .Where(x => x.FileName == "EmbeddedMp3.mp3")
                .Select(x => x.EmbeddedFile)
                .First();

            Assert.AreEqual(embeddedPdfSha256, GetHashSHA256(extractedFile));
        }

        [TestMethod()]
        public void ParseWordExtractInsertedTextTest()
        {
            string embeddedPdfSha256 = "BE34886C755E1E5A5D1A15474645FD1DD19974968E6599C9BC75725DE0F979D5";
            FileController fileController = new FileController(null!);
            byte[] extractedFile = files
                .Where(x => x.FileName == "TEST_INSERT")
                .Select(x => x.EmbeddedFile)
                .First();

            Assert.AreEqual(embeddedPdfSha256, GetHashSHA256(extractedFile));
        }

        [DataTestMethod]
        [DataRow("Microsoft Word Document", true)]
        [DataRow("Adobe Acrobat Document", true)]
        [DataRow("EmbeddedMp3.mp3", true)]
        [DataRow("EmbeddedPng.png", true)]
        [DataRow("TEST_INSERT", false)]
        [DataRow("BMP File", true)]
        [DataRow("Lorem ipsum dolor sit amet,", false)]
        public void TestHasIcons(string fileName, bool expected)
        {

            bool result = files
                .Where(x => x.FileName.StartsWith(fileName))
                .Select(x => !x.FileNameWarnings.Contains(StringConstants.WARNINGS.NO_EXPLICIT_NAME))
                .First();

            Assert.AreEqual(expected, result);
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