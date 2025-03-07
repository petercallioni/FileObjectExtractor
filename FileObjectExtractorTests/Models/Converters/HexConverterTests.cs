using FileObjectExtractor.Converters;

namespace FileObjectExtractor.Models.Converters.Tests
{
    [TestClass()]
    public class HexConverterTests
    {
        [TestMethod()]
        public void LittleEndianHexToIntTest()
        {
            byte[] data = { 0x10, 0x00, 0x00, 0x00 };
            Assert.IsTrue(HexConverter.LittleEndianHexToInt(data) == 16);
        }

        [TestMethod()]
        public void LittleEndianHexToUIntTest()
        {
            byte[] data = { 0x10, 0x00, 0x00, 0x00 };
            Assert.IsTrue(HexConverter.LittleEndianHexToUInt(data) == 16);
        }

        [TestMethod()]
        public void LittleEndianHexToFloatTest()
        {
            byte[] data = { 0x00, 0x00, 0x84, 0x41 };
            Assert.IsTrue(HexConverter.LittleEndianHexToFloat(data) == 16.5f);
        }

        [TestMethod()]
        public void HexToStringTest()
        {
            byte[] data = {
                0x45, 0x00, 0x6D, 0x00, 0x62, 0x00, 0x65, 0x00,
                0x64, 0x00, 0x64, 0x00, 0x65, 0x00, 0x64, 0x00,
                0x54, 0x00, 0x65, 0x00, 0x73, 0x00, 0x74, 0x00,
                0x44, 0x00, 0x6F, 0x00, 0x63, 0x00, 0x78, 0x00
            };
            Assert.IsTrue(HexConverter.HexToString(data).Equals("EmbeddedTestDocx"));
        }
    }
}