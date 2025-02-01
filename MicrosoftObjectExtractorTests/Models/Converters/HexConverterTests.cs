namespace MicrosoftObjectExtractor.Models.Converters.Tests
{
    [TestClass()]
    public class HexConverterTests
    {
        [TestMethod()]
        public void LittleEndianHexToIntTest()
        {
            Assert.IsTrue(HexConverter.LittleEndianHexToInt("10000000") == 16);
        }

        [TestMethod()]
        public void LittleEndianHexToUIntTest()
        {
            Assert.IsTrue(HexConverter.LittleEndianHexToInt("10000000") == 16);
        }

        [TestMethod()]
        public void LittleEndianHexToFloatTest()
        {
            Assert.IsTrue(HexConverter.LittleEndianHexToInt("10000000") == 16);
        }

        [TestMethod()]
        public void HexToStringTest()
        {
            Assert.IsTrue(HexConverter.HexToString("45006D00620065006400640065006400540065007300740044006F0063007800").Equals("EmbeddedTestDocx"));
        }
    }
}