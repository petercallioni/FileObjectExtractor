namespace FileObjectExtractorTests
{
    internal static class TestResources
    {
        internal static Uri RESOURCES_DIRECTORY = new Uri(Path.GetFullPath("./Resources/"));
        internal static Uri DOCX = new Uri(Path.Combine(RESOURCES_DIRECTORY.AbsoluteUri, "TestDocx.docx"));
        internal static Uri DOCX_SPACES = new Uri(Path.Combine(RESOURCES_DIRECTORY.AbsoluteUri, "Document With Space.docx"));
        internal static Uri DOCX_ENCRYPTED = new Uri(Path.Combine(RESOURCES_DIRECTORY.AbsoluteUri, "TestDocxEncrypted.docx"));
        internal static Uri EXCEL = new Uri(Path.Combine(RESOURCES_DIRECTORY.AbsoluteUri, "TestExcel.xlsx"));
        internal static Uri POWERPOINT = new Uri(Path.Combine(RESOURCES_DIRECTORY.AbsolutePath, "TestPowerpoint.pptx"));

        internal static class EMBEDDED_FILES
        {
            internal static string EMBEDDED_DOCX = Path.Combine("EmbeddedTestDocx.docx");
            internal static string EMBEDDED_PDF = Path.Combine("EmbeddedTestPDF.pdf");
            internal static string INSERTED_DOCX = Path.Combine("NoIconDocx.docx");
            internal static string EMBEDDED_PNG = Path.Combine("EmbeddedPng.png");
            internal static string EMBEDDED_MP3 = Path.Combine("EmbeddedMp3.mp3");
            internal static string EMBEDDED_JSON = Path.Combine("EmbeddedJson.json");
        }

        internal static class EXTRACTION_TEST
        {
            internal static Uri EXTRACT_TEST_DIR = new Uri(Path.Combine(RESOURCES_DIRECTORY.AbsolutePath, "ExtractTestDir"));
            internal static string EXTRACT_TEST_DOCX = EMBEDDED_FILES.EMBEDDED_DOCX;
            internal static string EXTRACT_TEST_NEW_NAME = "NEW_TEST.docx";
        }
    }
}