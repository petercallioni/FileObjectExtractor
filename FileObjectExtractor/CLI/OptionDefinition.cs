using System;

namespace FileObjectExtractor.CLI
{
    public class OptionDefinition
    {
        private string[] aliases;
        private int parameterCount;
        private Action<ParsedOptions, string[]> apply;

        public string[] Aliases { get => aliases; set => aliases = value; }
        public int ParameterCount { get => parameterCount; set => parameterCount = value; }
        public Action<ParsedOptions, string[]> Apply { get => apply; set => apply = value; }

        public OptionDefinition(string[] aliases, int parameterCount, Action<ParsedOptions, string[]> apply)
        {
            this.aliases = aliases;
            this.parameterCount = parameterCount;
            this.apply = apply;
        }
    }
}
