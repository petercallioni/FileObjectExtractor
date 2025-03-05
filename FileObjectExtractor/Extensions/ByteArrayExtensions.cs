using System;

namespace FileObjectExtractor.Extensions
{
    public static class ByteArrayExtensions
    {
        public static bool ContainsSequence(byte[] haystack, byte[] needle)
        {
            if (needle.Length == 0) return true; // An empty array is always contained.
            if (haystack.Length < needle.Length) return false;

            Span<byte> haySpan = haystack;
            ReadOnlySpan<byte> needSpan = needle;

            for (int i = 0; i <= haystack.Length - needle.Length; i++)
            {
                if (haySpan.Slice(i, needle.Length).SequenceEqual(needSpan))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
