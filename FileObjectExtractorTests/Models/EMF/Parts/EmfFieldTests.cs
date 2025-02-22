namespace FileObjectExtractor.Models.EMF.EmfPart.Tests
{
    [TestClass()]
    public class EmfFieldTests
    {
        private byte[] testData = {
            0x54,
            0x00,
            0x00,
            0x00,
            0x12,
            0x34,
        };

        [TestMethod()]
        public void InitializeIntTest()
        {
            EmfField<int> field = new();
            field.ByteLength = 4;
            field.Initialize(new Queue<byte>(testData));
            Assert.AreEqual(84, field.Value);
        }

        [TestMethod()]
        public void InitializeStringTest()
        {
            EmfField<string> field = new();
            field.ByteLength = 2;
            field.Initialize(new Queue<byte>(testData));
            Assert.AreEqual("T", field.Value);
        }
    }
}