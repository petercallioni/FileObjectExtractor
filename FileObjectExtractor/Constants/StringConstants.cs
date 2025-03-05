namespace FileObjectExtractor.Constants
{
    public static class StringConstants
    {
        public const string DEFAULT_DROP_TEXT = "Drop\n+\nHere";

        public static class WARNINGS
        {
            public const string NO_EXPLICIT_NAME = "Embedded file does not have an explicit file name.";
            public const string NO_EXTENSION = "File does not appear to have an extension.";
            public const string INVALID_CHARACTERS = "Filename has invalid characters for your operating system. These will be replaced with an underscore \"_\".";
            public const string LONG_FILENAME = $"This filename will be truncated.";
        }
    }
}
