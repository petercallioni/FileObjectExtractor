using System.Text;

namespace FileObjectExtractor.Extensions.Tests
{
    [TestClass()]
    public class StringBuilderExtensionsTests
    {
        private StringBuilder testStringBuilder = new StringBuilder("HelloWorld");

        [TestMethod()]
        public void Shift_NormalUsage_Test()
        {
            int startIndex = 5;
            int length = 5;
            string removedPart = testStringBuilder.Shift(startIndex, length);
            string result = testStringBuilder.ToString();
            Assert.AreEqual("World", removedPart);
            Assert.AreEqual("Hello", result);
        }

        [TestMethod()]
        public void Shift_ZeroLength_Test()
        {
            int startIndex = 5;
            int length = 0;
            string removedPart = testStringBuilder.Shift(startIndex, length);
            string result = testStringBuilder.ToString();
            Assert.AreEqual("", removedPart);
            Assert.AreEqual("HelloWorld", result);
        }

        [TestMethod()]
        public void Shift_StartIndexZero_Test()
        {
            int startIndex = 0;
            int length = 5;
            string removedPart = testStringBuilder.Shift(startIndex, length);
            string result = testStringBuilder.ToString();
            Assert.AreEqual("Hello", removedPart);
            Assert.AreEqual("World", result);
        }

        [TestMethod()]
        public void Shift_EndOfBuilder_Test()
        {
            int startIndex = 5;
            int length = 5;
            string removedPart = testStringBuilder.Shift(startIndex, length);
            string result = testStringBuilder.ToString();
            Assert.AreEqual("World", removedPart);
            Assert.AreEqual("Hello", result);
        }

        [TestMethod()]
        public void Shift_FullLength_Test()
        {
            int startIndex = 0;
            int length = testStringBuilder.Length;
            string removedPart = testStringBuilder.Shift(startIndex, length);
            string result = testStringBuilder.ToString();
            Assert.AreEqual("HelloWorld", removedPart);
            Assert.AreEqual("", result);
        }

        [TestMethod()]
        public void Shift_InvalidStartIndex_Test()
        {
            int startIndex = 10; // Out of bounds
            int length = 5;
            Assert.ThrowsException<System.ArgumentOutOfRangeException>(() => testStringBuilder.Shift(startIndex, length));
        }

        [TestMethod()]
        public void Shift_InvalidLength_Test()
        {
            int startIndex = 5;
            int length = 10; // Exceeds the remaining length
            Assert.ThrowsException<System.ArgumentOutOfRangeException>(() => testStringBuilder.Shift(startIndex, length));
        }

        [TestMethod()]
        public void Shift_NegativeStartIndex_Test()
        {
            int startIndex = -1;
            int length = 5;
            Assert.ThrowsException<System.ArgumentOutOfRangeException>(() => testStringBuilder.Shift(startIndex, length));
        }

        [TestMethod()]
        public void Shift_NegativeLength_Test()
        {
            int startIndex = 5;
            int length = -1;
            Assert.ThrowsException<System.ArgumentOutOfRangeException>(() => testStringBuilder.Shift(startIndex, length));
        }
    }
}