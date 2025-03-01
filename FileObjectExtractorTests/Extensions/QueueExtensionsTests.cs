namespace FileObjectExtractor.Extensions.Tests
{
    [TestClass()]
    public class QueueExtensionsTests
    {
        private Queue<int> testQueue = new Queue<int>(new[] { 1, 2, 3, 4, 5 });

        [TestMethod()]
        public void DequeueMultiple_NormalUsage_Test()
        {
            // Arrange
            int count = 3;

            // Act
            int[] result = testQueue.DequeueMultiple(count);

            // Assert
            CollectionAssert.AreEqual(new int[] { 1, 2, 3 }, result);
            Assert.AreEqual(2, testQueue.Count);
        }

        [TestMethod()]
        public void DequeueMultiple_ZeroElements_Test()
        {
            // Arrange
            int count = 0;

            // Act
            int[] result = testQueue.DequeueMultiple(count);

            // Assert
            CollectionAssert.AreEqual(new int[0], result);
            Assert.AreEqual(5, testQueue.Count);
        }

        [TestMethod()]
        public void DequeueMultiple_AllElements_Test()
        {
            // Arrange
            int count = 5;

            // Act
            int[] result = testQueue.DequeueMultiple(count);

            // Assert
            CollectionAssert.AreEqual(new int[] { 1, 2, 3, 4, 5 }, result);
            Assert.AreEqual(0, testQueue.Count);
        }

        [TestMethod()]
        public void DequeueMultiple_EmptyQueue_Test()
        {
            // Arrange
            Queue<int> emptyQueue = new Queue<int>();
            int count = 2;

            // Act & Assert
            Assert.ThrowsException<System.ArgumentOutOfRangeException>(() => emptyQueue.DequeueMultiple(count));
        }

        [TestMethod()]
        public void DequeueMultiple_NegativeCount_Test()
        {
            // Arrange
            int count = -1;

            // Act & Assert
            Assert.ThrowsException<System.ArgumentOutOfRangeException>(() => testQueue.DequeueMultiple(count));
        }

        [TestMethod()]
        public void DequeueMultiple_CountGreaterThanQueueLength_Test()
        {
            // Arrange
            int count = 6; // Greater than the number of elements in the queue

            // Act & Assert
            Assert.ThrowsException<System.ArgumentOutOfRangeException>(() => testQueue.DequeueMultiple(count));
        }
    }
}