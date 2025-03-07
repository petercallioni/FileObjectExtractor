using FileObjectExtractor.Converters;
using System.Globalization;

namespace FileObjectExtractor.Models.Converters.Tests
{
    [TestClass]
    public class FileNameShortenerTests
    {
        [DataTestMethod]
        [DataRow("shortfilename.txt", "shortfilename.txt")]
        [DataRow("averyveryveryveryveryveryveryveryveryveryverylongfilename.txt", "averyveryveryveryveryveryveryveryveryveryverylongf...")]
        [DataRow(null, null)]
        public void Convert_ShouldShortenFileName_WhenFileNameIsTooLong(string input, string expected)
        {
            // Arrange
            FileNameShortener converter = new FileNameShortener();

            // Act
            object? result = converter.Convert(input, typeof(string), null, CultureInfo.InvariantCulture);

            // Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void ConvertBack_ShouldThrowNotImplementedException()
        {
            // Arrange
            FileNameShortener converter = new FileNameShortener();

            // Act & Assert
            Assert.ThrowsException<NotImplementedException>(() => converter.ConvertBack(null, typeof(string), null, CultureInfo.InvariantCulture));
        }
    }
}
