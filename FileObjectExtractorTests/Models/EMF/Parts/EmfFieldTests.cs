using System.Text;

namespace FileObjectExtractor.Models.EMF.EmfPart.Tests
{
    [TestClass()]
    public class EmfFieldTests
    {
        [TestMethod()]
        public void InitializeIntTest()
        {
            EmfField<int> field = new();
            field.ByteLength = 4;
            field.Initialize(new StringBuilder("540000001234"));
        }

        [TestMethod()]
        public void InitializeFloatTest()
        {
            EmfField field = new EmfField();
            Assert.Fail();
        }

        [TestMethod()]
        public void InitializeStringTest()
        {
            EmfField field = new EmfField();
            Assert.Fail();
        }
    }
}