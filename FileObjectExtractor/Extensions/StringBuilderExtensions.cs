using System.Text;

namespace FileObjectExtractor.Extensions
{
    public static class StringBuilderExtensions
    {
        public static string Shift(this StringBuilder sb, int startIndex, int length)
        {
            // Store the part to be removed
            string removedPart = sb.ToString(startIndex, length);

            // Remove the part from the StringBuilder
            sb.Remove(startIndex, length);

            // Return the removed part
            return removedPart;
        }
    }
}
