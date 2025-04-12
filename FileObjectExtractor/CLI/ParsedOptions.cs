using System.Collections.Generic;

namespace FileObjectExtractor.CLI
{
    public class ParsedOptions
    {
        // Flags
        public bool Help { get; set; } = false;
        public bool Version { get; set; } = false;
        public bool List { get; set; } = false;

        // Options with a parameter
        public string? InputFilename { get; set; }
        public string? OutputDirectory { get; set; }
        public string? SelectItem { get; set; }
        public string? CustomName { get; set; }

        // Any extra, positional arguments can be captured here
        public List<string> PositionalArguments { get; } = new List<string>();
    }
}
