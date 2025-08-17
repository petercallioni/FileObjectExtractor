using System;

namespace FileObjectExtractor.Extensions
{
    public static class UriExtensions
    {
        public static string UnescapedString(this Uri uri)
        {
            if (uri == null)
                throw new ArgumentNullException(nameof(uri));

            if (!uri.IsFile)
                throw new ArgumentException("The provided URI is not a file URI.", nameof(uri));

            // For UNC paths, LocalPath returns the correct \\server\share\path format on Windows
            // For Linux, LocalPath returns /path/to/file
            // For Windows local files, returns C:\path\to\file
            return uri.LocalPath;
        }
    }
}
