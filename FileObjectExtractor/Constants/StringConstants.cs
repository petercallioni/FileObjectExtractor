﻿namespace FileObjectExtractor.Constants
{
    public static class StringConstants
    {
        public const string DEFAULT_DROP_TEXT = "Drop\n+\nHere";
        public const string TEMP_FILE_EXTENSION = ".fox_old";

        public static class WARNINGS
        {
            public const string NO_EXPLICIT_NAME = "File embedded as content: file may not have a file name.";
            public const string GUESSED_EXTENSION = "The extension was guessed from the file contents: it may not be correct.";
            public const string NO_EXTENSION = "Original file name does not appear to have an extension.";
            public const string INVALID_CHARACTERS = "Original file name has invalid characters for your operating system. These have been replaced with an underscore \"_\".";
            public const string LONG_FILENAME = "Original file name has been truncated.";
            public const string LINKED_FILE = "This is a linked file and can only be extracted if the linked file can be found.";
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
  -u, -check-for-update,                Check if a new version of the application is available.
  --check-for-update                    Prompt to install the update if available.

  -uy, -check-for-update-and-install,   Check for updates and automatically install if available.
  --check-for-update-and-install        

Examples:
  [application-name] -i input.docx -o output/
  [application-name] -i input.docx -l
  [application-name] -i input.docx -o output/ -s 3
  [application-name] -i input.docx -o output/ -s 3 -n newfile.txt
  [application-name] -u
  [application-name] -uy

Notes:
  - Options can be combined in various orders.
  - The -i (input) option is mandatory for any operation except update checks.
  - Ensure the output directory exists before running the extraction.

";
    }
}
