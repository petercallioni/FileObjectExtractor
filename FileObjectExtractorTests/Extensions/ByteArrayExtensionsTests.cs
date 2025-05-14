namespace FileObjectExtractor.Extensions.Tests
{
    [TestClass]
    public class ByteArrayExtensionsTests
    {
        [TestMethod]
        public void ContainsSequence_EmptyNeedle_ReturnsTrue()
        {
            byte[] haystack = { 0x01, 0x02, 0x03 };
            byte[] needle = Array.Empty<byte>();

            bool result = haystack.ContainsSequence(needle);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void ContainsSequence_EmptyHaystack_ReturnsFalse()
        {
            byte[] haystack = Array.Empty<byte>();
            byte[] needle = { 0x01 };

            bool result = haystack.ContainsSequence(needle);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void ContainsSequence_NeedleLongerThanHaystack_ReturnsFalse()
        {
            byte[] haystack = { 0x01, 0x02 };
            byte[] needle = { 0x01, 0x02, 0x03 };

            bool result = haystack.ContainsSequence(needle);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void ContainsSequence_SequenceFoundAtStart_ReturnsTrue()
        {
            byte[] haystack = { 0x01, 0x02, 0x03 };
            byte[] needle = { 0x01, 0x02 };

            bool result = haystack.ContainsSequence(needle);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void ContainsSequence_SequenceFoundInMiddle_ReturnsTrue()
        {
            byte[] haystack = { 0x01, 0x02, 0x03, 0x04 };
            byte[] needle = { 0x02, 0x03 };

            bool result = haystack.ContainsSequence(needle);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void ContainsSequence_SequenceFoundAtEnd_ReturnsTrue()
        {
            byte[] haystack = { 0x01, 0x02, 0x03 };
            byte[] needle = { 0x02, 0x03 };

            bool result = haystack.ContainsSequence(needle);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void ContainsSequence_SequenceNotFound_ReturnsFalse()
        {
            byte[] haystack = { 0x01, 0x02, 0x03 };
            byte[] needle = { 0x04 };

            bool result = haystack.ContainsSequence(needle);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void ContainsSequence_HaystackEqualsNeedle_ReturnsTrue()
        {
            byte[] haystack = { 0x01, 0x02, 0x03 };
            byte[] needle = { 0x01, 0x02, 0x03 };

            bool result = haystack.ContainsSequence(needle);

            Assert.IsTrue(result);
        }
    }
}