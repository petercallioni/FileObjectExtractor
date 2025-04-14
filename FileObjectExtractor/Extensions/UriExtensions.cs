using System;

namespace FileObjectExtractor.Extensions
{
    public static class UriExtensions
    {
        public static string UnescapedString(this Uri uri)
        {
            // Decode the URI to get the unescaped string
            string unescapedString = Uri.UnescapeDataString(uri.ToString());
            // Remove the "file:///" prefix if present
            if (unescapedString.StartsWith("file:///"))
            {
                unescapedString = unescapedString.Substring(8);
            }

            return unescapedString;
        }
    }
}
