namespace FileObjectExtractorTests
{
    internal static class TestResources
    {
        internal static string RESOURCES_DIRECTORY = Path.GetFullPath("./Resources/");
        internal static string DOCX = Path.Combine(RESOURCES_DIRECTORY, "TestDocx.docx");
        internal static string EXCEL = Path.Combine(RESOURCES_DIRECTORY, "TestExcel.xlsx");
        internal static string POWERPOINT = Path.Combine(RESOURCES_DIRECTORY, "TestPowerpoint.pptx");

        internal static class EMBEDDED_FILES
        {
            internal static string EMBEDDED_DOCX = Path.Combine("EmbeddedTestDocx.docx");
            internal static string EMBEDDED_PDF = Path.Combine("EmbeddedTestPDF.pdf");
            internal static string INSERTED_DOCX = Path.Combine("NoIconDocx.docx");
            internal static string EMBEDDED_PNG = Path.Combine("EmbeddedPng.png");
            internal static string EMBEDDED_MP3 = Path.Combine("EmbeddedMp3.mp3");
            internal static string EMBEDDED_JSON = Path.Combine("EmbeddedJson.json");
        }
    }
}