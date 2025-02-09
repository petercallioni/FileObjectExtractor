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
            (int byteSize, string expected)[] testCases = new (int byteSize, string expected)[]
            {
                (0, "0 B"),
                (512, "512 B"),
                (1023, "1023 B"),
                (1024, "1 KiB"),
                (2048, "2 KiB"),
                (1048576, "1 MiB"),
                (1073741824, "1 GiB"),
            };

            foreach ((int byteSize, string expected) in testCases)
            {
                byte[] bytes = new byte[byteSize];
                // Act
                object result = _converter.Convert(bytes, typeof(string), null, CultureInfo.InvariantCulture);

                // Assert
                Assert.AreEqual(expected, result);
            }
        }
    }
}