using System;
using System.Collections.Generic;

namespace FileObjectExtractor.Extensions
{
    public static class QueueExtensions
    {
        public static T[] DequeueMultiple<T>(this Queue<T> queue, int count)
        {
            if (count < 0 || count > queue.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(count), "Count must be non-negative and less than or equal to the number of elements in the queue.");
            }

            T[] result = new T[count];
            for (int i = 0; i < count; i++)
            {
                result[i] = queue.Dequeue();
            }

            return result;
        }
    }
}
