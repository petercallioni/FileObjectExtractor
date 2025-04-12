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
            public const string LONG_FILENAME = "This filename will be truncated.";
        }

        public static class CLI_ERRORS
        {
            public const string NO_INPUT_FILE = "The option given requires a input file.";
            public const string NO_OUTPUT_DIRECTORY = "A valid output directory must be given.";
            public const string INPUT_FILE_NOT_FOUND = "Given input file not found. Aborting.";
            public const string OUTPUT_DIRECTORY_NOT_FOUND = "Given output directory not found. Aborting.";
            public const string NO_FILES_EXTRACTED = "No files were extracted. Either the document contains no extractable files, or if -s was sepecificed, the file with the given index or filename was not found.";
            public const string NO_FILES_SELECTED = "Use of -n requires -s to select a given file.";
        }

        public const string HELP_TEXT = @"
Usage: [application-name] [options]

Options:
  -h, -help, --help                     Display this help message.
  -v, -version, --version               Display the application version.
  -i, -input, --input [file/path]       Specify the input file containing the files to extract. 
                                        This option is required.
  -o, -output, --output [directory]     Specify the output directory to place the extracted files. 
                                        This option is required with -s and -n.
  -l, -list, --list                     Display the list of files available to extract in the format:
                                        ""<id> - <file name>"". 
                                        Requires the -i option.
  -s, -select, --select [id/file name]  Select a file from the list to extract. If omitted, all files will 
                                        be extracted. Can be identified using <id> or <file name>. 
                                        Requires -i and -o options.
  -n, -name, --name [new file name]     Save the selected file with a new name. Used in conjunction with 
                                        -s. Requires -i, -o, and -s options.

Examples:
  [application-name] -i input.docx -o output/
  [application-name] -i input.docx -l
  [application-name] -i input.docx -o output/ -s 3
  [application-name] -i input.docx -o output/ -s 3 -n newfile.txt

Notes:
  - Options can be combined in various orders.
  - The -i (input) option is mandatory for any operation.
  - Ensure the output directory exists before running the extraction.

";
    }
}
