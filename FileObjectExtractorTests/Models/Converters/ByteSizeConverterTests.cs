using System.Globalization;

namespace FileObjectExtractor.Models.Converters.Tests
{
    [TestClass()]
    public class ByteSizeConverterTests
    {
        ByteSizeConverter _converter;

        public ByteSizeConverterTests()
        {
            _converter = new ByteSizeConverter();
        }

        [TestMethod()]
        public void ConvertTest()
        {
            (byte[] byteArray, string expected)[] testCases = new (byte[] byteArray, string expected)[]
            {
                (new byte[0], "0 B"),
                (new byte[512], "512 B"),
                (new byte[1023], "1023 B"),
                (new byte[1024], "1 KiB"),
                (new byte[2048], "2 KiB"),
                (new byte[1048576], "1 MiB"),
                (new byte[1073741824], "1 GiB"),
            };

            foreach ((byte[] byteArray, string expected) in testCases)
            {
                // Act
                object result = _converter.Convert(byteArray, typeof(string), null, CultureInfo.InvariantCulture);

                // Assert
                Assert.AreEqual(expected, result);
            }
        }
    }
}