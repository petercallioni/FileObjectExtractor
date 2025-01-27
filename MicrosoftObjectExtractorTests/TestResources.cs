namespace MicrosoftObjectExtractorTests
{
    internal static class TestResources
    {
        internal static string RESOURCES_DIRECTORY = Path.GetFullPath("./Resources/");
        internal static string DOCX = Path.Combine(RESOURCES_DIRECTORY, "TestDocx.docx");

        internal static class DOCX_EMBEDDED_FILES
        {
            internal static string EMBEDDED_DOCX = Path.Combine("EmbeddedTestDocx.docx");
            internal static string EMBEDDED_PDF = Path.Combine("EmbeddedTestPDF.pdf");
        }
    }
}