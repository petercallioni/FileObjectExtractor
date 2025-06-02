using FileObjectExtractor.Constants;
using FileObjectExtractor.Services;
using FileObjectExtractor.Updates;
using FileObjectExtractorTests;
using FileObjectExtractorTests.Models;
using System.Reflection;
using System.Text.RegularExpressions;

namespace FileObjectExtractor.CLI.Tests
{
    [TestClass()]
    public class CommandLineParserTests
    {
        private record ReturnResult(ExitCode ExitCode, string OutputText);

        private TextWriter originalOutput = null!;

        // We need the following as tests interfere with the console output frokm each other

        [TestInitialize]
        public void TestInitialize()
        {
            // Save the original output so we can restore it later.
            originalOutput = Console.Out;
            if (Directory.Exists(TestResources.EXTRACTION_TEST.EXTRACT_TEST_DIR.AbsolutePath))
            {
                Directory.Delete(TestResources.EXTRACTION_TEST.EXTRACT_TEST_DIR.AbsolutePath, true);
            }

            Directory.CreateDirectory(TestResources.EXTRACTION_TEST.EXTRACT_TEST_DIR.AbsolutePath);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            // Restore the original output after each test.
            Console.SetOut(originalOutput);
            Directory.Delete(TestResources.EXTRACTION_TEST.EXTRACT_TEST_DIR.AbsolutePath, true);
        }

        [TestMethod()]
        [DataTestMethod]
        [DataRow("-h")]
        [DataRow("-help")]
        [DataRow("--help")]
        [DataRow("-z")]
        public void ParseTest_HelpArguments(string input)
        {
            ExitCode expectedExitCode = ExitCode.SUCCESS;
            string expectedText = StringConstants.HELP_TEXT;

            string[] args = input.Split(" ");
            ReturnResult returnResult = GetConsoleOutput(args);

            Assert.AreEqual(expectedExitCode, returnResult.ExitCode);
            Assert.AreEqual(expectedText.Trim(), returnResult.OutputText.Trim());
        }

        [TestMethod()]
        [DataTestMethod]
        [DataRow("-v")]
        [DataRow("-version")]
        [DataRow("--version")]
        public void ParseTest_VersionArguments(string input)
        {
            string versionNumber = Assembly.GetExecutingAssembly()?.GetName()?.Version?.ToString() ?? "Unknown";
            string expectedText = ($"{versionNumber}");

            Regex regex = new Regex(@"\d+\.\d+\.\d+");

            string[] args = input.Split(" ");
            ReturnResult returnResult = GetConsoleOutput(args);

            Assert.IsTrue(regex.IsMatch(returnResult.OutputText));
        }

        [TestMethod()]
        [DataTestMethod]
        [DataRow("-l")]
        [DataRow("-list")]
        [DataRow("--list")]
        public void ParseTest_List(string input)
        {
            string[] args = [input, TestResources.DOCX.AbsolutePath];
            ReturnResult returnResult = GetConsoleOutput(args);

            Assert.AreEqual("0 - EmbeddedTestDocx.docx\r\n1 - EmbeddedTestPDF.pdf\r\n2 - EmbeddedJson.json\r\n3 - EmbeddedPng.png\r\n4 - EmbeddedMp3.mp3\r\n5 - TEST_INSERT.docx\r\n6 - EmbeddedPng.bmp\r\n7 - Docx Test.docx\r\n8 - Lorem ipsum dolor sit amet, consectetur adipiscing...", returnResult.OutputText);
        }

        [TestMethod()]
        public void ParseTest_Input_Output_No_Output()
        {
            string[] args = ["-i", TestResources.DOCX.AbsolutePath];
            ReturnResult returnResult = GetConsoleOutput(args);

            Assert.AreEqual(ExitCode.ERROR, returnResult.ExitCode);
            Assert.AreEqual(StringConstants.CLI_ERRORS.NO_OUTPUT_DIRECTORY, returnResult.OutputText);
        }

        [TestMethod()]
        public void ParseTest_Input_Output_OutputDirNotFound()
        {
            string[] args = ["-i", TestResources.DOCX.AbsolutePath, "-o", "NON_EXISTANT_DIR"];
            ReturnResult returnResult = GetConsoleOutput(args);

            Assert.AreEqual(ExitCode.ERROR, returnResult.ExitCode);
            Assert.AreEqual(StringConstants.CLI_ERRORS.OUTPUT_DIRECTORY_NOT_FOUND, returnResult.OutputText);
        }

        [TestMethod()]
        [DataRow("-i", "-o")]
        [DataRow("-input", "-output")]
        [DataRow("--input", "--output")]
        public void ParseTest_Input_Output_Output(string input, string output)
        {
            string[] args = [input, TestResources.DOCX.AbsolutePath, output, TestResources.EXTRACTION_TEST.EXTRACT_TEST_DIR.AbsolutePath];
            ReturnResult returnResult = GetConsoleOutput(args);

            Assert.AreEqual(ExitCode.SUCCESS, returnResult.ExitCode);
            Assert.AreEqual("Extracting EmbeddedTestDocx.docx\r\nExtracting EmbeddedTestPDF.pdf\r\nExtracting EmbeddedJson.json\r\nExtracting EmbeddedPng.png\r\nExtracting EmbeddedMp3.mp3\r\nExtracting TEST_INSERT.docx\r\nExtracting EmbeddedPng.bmp\r\nExtracting Docx Test.docx\r\nExtracting Lorem ipsum dolor sit amet, consectetur adipiscing...", returnResult.OutputText);
        }

        [TestMethod()]
        [DataRow("-i", "-o", "-s", "2", "Extracting EmbeddedJson.json")]
        [DataRow("-i", "-o", "-s", "EmbeddedPng.png", "Extracting EmbeddedPng.png")]
        public void ParseTest_Input_Output_Selective(string input, string output, string selective, string selectiveItem, string expectedOutput)
        {
            string[] args = [input, TestResources.DOCX.AbsolutePath, output, TestResources.EXTRACTION_TEST.EXTRACT_TEST_DIR.AbsolutePath, selective, selectiveItem];
            ReturnResult returnResult = GetConsoleOutput(args);

            Assert.AreEqual(ExitCode.SUCCESS, returnResult.ExitCode);
            Assert.AreEqual(expectedOutput, returnResult.OutputText);
        }

        [TestMethod()]
        [DataRow("-i", "-o", "-s", "2", "-n", "JSON FILE.json", "Extracting EmbeddedJson.json as JSON FILE.json")]
        [DataRow("-i", "-o", "-s", "EmbeddedPng.png", "-n", "PICTURE FILE.jpg", "Extracting EmbeddedPng.png as PICTURE FILE.jpg")]
        public void ParseTest_Input_Output_Named(string input, string output, string selective, string selectiveItem, string nameFlag, string name, string expectedOutput)
        {
            string[] args = [input, TestResources.DOCX.AbsolutePath, output, TestResources.EXTRACTION_TEST.EXTRACT_TEST_DIR.AbsolutePath, selective, selectiveItem, nameFlag, name];
            ReturnResult returnResult = GetConsoleOutput(args);

            Assert.AreEqual(ExitCode.SUCCESS, returnResult.ExitCode);
            Assert.AreEqual(expectedOutput, returnResult.OutputText);
        }

        private ReturnResult GetConsoleOutput(string[] args)
        {
            MockFileController mockFileController = new MockFileController();
            CliController cliController = new CliController(args, mockFileController, new UpdateService(), new CliProgressService());
            string returnText;
            ExitCode exitCode;

            using (StringWriter output = new StringWriter())
            {
                Console.SetOut(output);

                exitCode = cliController.StartCLI();
                returnText = output.ToString().Trim();
            }

            return new ReturnResult(exitCode, returnText);
        }
    }
}