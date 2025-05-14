public static class ByteSizeFormatter
{
    private static readonly string[] SizeSuffixes = { "B", "KiB", "MiB", "GiB", "TiB" };

    /// <summary>
    /// Converts a byte count to a human-readable string.
    /// </summary>
    public static string Format(long byteCount)
    {
        double size = byteCount;
        int order = 0;

        while (size >= 1024 && order < SizeSuffixes.Length - 1)
        {
            order++;
            size /= 1024;
        }

        return string.Format("{0:0.##} {1}", size, SizeSuffixes[order]);
    }

    /// <summary>
    /// Convenience method accepting a byte array.
    /// </summary>
    public static string Format(byte[] byteArray)
    {
        if (byteArray == null)
            return "0 B";

        return Format(byteArray.Length);
    }
}