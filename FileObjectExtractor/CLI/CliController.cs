using FileObjectExtractor.Constants;
using FileObjectExtractor.Models;
using FileObjectExtractor.Models.Office;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace FileObjectExtractor.CLI
{
    public class CliController
    {
        private readonly ParsedOptions options;
        private readonly IFileController fileCcontroller;
        public CliController(string[] args, IFileController fileController)
        {
            CommandLineParser commandLineParser = new CommandLineParser();
            options = commandLineParser.Parse(args);
            this.fileCcontroller = fileController;
        }

        public ExitCode StartCLI()
        {
            ExitCode exitCode;

            if (options.PositionalArguments.Count > 0) // We have arguments we do not know what to do with
            {
                return DisplayHelp();
            }

            if (options.Help)
            {
                return DisplayHelp();
            }

            if (options.Version)
            {
                return DisplayVersion();
            }

            if (options.List)
            {
                return ListItems();
            }

            if (!options.List && ValidateInputFile(out exitCode))
            {
                return ProcessInputFile(options.SelectItem, options.CustomName);
            }

            return ExitCode.UNKNOWN;
        }

        private ExitCode DisplayHelp()
        {
            Console.WriteLine(StringConstants.HELP_TEXT);
            return ExitCode.SUCCESS;
        }

        private ExitCode DisplayVersion()
        {
            string buildDate = System.IO.File.GetLastWriteTime(Assembly.GetExecutingAssembly().Location).ToString("yyyy-MM-dd");
            string versionNumber = Assembly.GetExecutingAssembly()?.GetName()?.Version?.ToString() ?? "Unknown";
            Console.WriteLine($"{versionNumber} ({buildDate})");
            return ExitCode.SUCCESS;
        }

        private ExitCode ListItems()
        {
            if (!ValidateInputFile(out ExitCode exitCode))
            {
                return exitCode;
            }

            Uri fileUri = new Uri(options.InputFilename!);
            IParseOffice parseOffice = OfficeParserPicker.GetOfficeParser(fileUri);

            List<ExtractedFile> embeddedFiles = parseOffice.GetExtractedFiles(fileUri);

            foreach (ExtractedFile embeddedFile in embeddedFiles)
            {
                Console.WriteLine($"{embeddedFile.DocumentOrder} - {embeddedFile.SafeFileName}");
            }

            return ExitCode.SUCCESS;
        }

        private ExitCode ProcessInputFile(string? selectItem, string? customName)
        {
            ExitCode exitCode = ExitCode.UNKNOWN;

            if (!ValidateOutputDirectory(out exitCode))
            {
                return exitCode;
            }

            Uri fileUri = new Uri(options.InputFilename!);
            IParseOffice parseOffice = OfficeParserPicker.GetOfficeParser(fileUri);
            List<ExtractedFile> embeddedFiles = parseOffice.GetExtractedFiles(fileUri);

            int outputFileCount = 0;

            foreach (ExtractedFile embeddedFile in embeddedFiles)
            {
                if (options.SelectItem != null)
                {
                    bool matchFound = false;
                    bool validDocumentNunber = int.TryParse(options.SelectItem, out int index);

                    if (validDocumentNunber)
                    {
                        if (embeddedFile.DocumentOrder == index)
                        {
                            matchFound = true;
                        }
                    }
                    else if (embeddedFile.SafeFileName.Equals(options.SelectItem, StringComparison.OrdinalIgnoreCase))
                    {
                        matchFound = true;
                    }

                    if (matchFound)
                    {
                        HandleFile(fileCcontroller, embeddedFile, customName);
                        outputFileCount++;
                    }
                }
                else
                {
                    // No specific item selected, handle all files
                    HandleFile(fileCcontroller, embeddedFile);
                    outputFileCount++;
                }
            }

            if (outputFileCount == 0)
            {
                Console.WriteLine(StringConstants.CLI_ERRORS.NO_FILES_EXTRACTED);
                return ExitCode.ERROR;
            }

            return ExitCode.SUCCESS;
        }

        private void HandleFile(IFileController fileController, ExtractedFile embeddedFile, string? customName = null)
        {
            string outputMessage = $"Extracting {embeddedFile.SafeFileName}";
            string fileName = embeddedFile.SafeFileName;

            if (customName != null)
            {
                fileName = customName;
                outputMessage += $" as {customName}";
            }

            Console.WriteLine(outputMessage);
            fileCcontroller.SaveFile(Path.Combine(options.OutputDirectory!, fileName), embeddedFile);
        }

        private bool ValidateInputFile(out ExitCode exitCode)
        {
            exitCode = ExitCode.SUCCESS;

            if (string.IsNullOrEmpty(options.InputFilename))
            {
                Console.WriteLine(StringConstants.CLI_ERRORS.NO_INPUT_FILE);
                exitCode = ExitCode.ERROR;
            }
            else if (!new FileInfo(options.InputFilename).Exists)
            {
                Console.WriteLine(StringConstants.CLI_ERRORS.INPUT_FILE_NOT_FOUND);
                exitCode = ExitCode.ERROR;
            }

            return exitCode == ExitCode.SUCCESS ? true : false;
        }

        private bool ValidateOutputDirectory(out ExitCode exitCode)
        {
            exitCode = ExitCode.SUCCESS;

            if (string.IsNullOrEmpty(options.OutputDirectory))
            {
                Console.WriteLine(StringConstants.CLI_ERRORS.NO_OUTPUT_DIRECTORY);
                exitCode = ExitCode.ERROR;
            }
            else if (!new DirectoryInfo(options.OutputDirectory).Exists)
            {
                Console.WriteLine(StringConstants.CLI_ERRORS.OUTPUT_DIRECTORY_NOT_FOUND);
                exitCode = ExitCode.ERROR;
            }

            return exitCode == ExitCode.SUCCESS ? true : false;
        }
    }

    public class CommandLineParser
    {
        public ParsedOptions Parse(string[] args)
        {
            ParsedOptions options = new ParsedOptions();

            // Define the available options here with their aliases.
            List<OptionDefinition> optionDefinitions = new List<OptionDefinition>
            {
                new OptionDefinition(["-h", "-help", "--help"], 0, (opts, _) => opts.Help = true),
                new OptionDefinition(["-v", "-version", "--version"], 0, (opts, _) => opts.Version = true),
                new OptionDefinition(["-i", "-input", "--input"], 1, (opts, parameters) => opts.InputFilename = parameters[0]),
                new OptionDefinition(["-l", "-list", "--list"], 1, (opts, parameters) =>
                {
                    opts.List=true;
                    opts.InputFilename = parameters[0];
                }),
                new OptionDefinition(["-n", "-name", "--name"], 1, (opts, parameters) => opts.CustomName = parameters[0]),
                new OptionDefinition(["-s", "-select", "--select"], 1, (opts, parameters) => opts.SelectItem = parameters[0]),
                new OptionDefinition(["-o", "-output", "--output"], 1, (opts, parameters) => opts.OutputDirectory = parameters[0])
            };

            // For fast lookup, create a dictionary that maps every alias to its definition.
            Dictionary<string, OptionDefinition> lookup = new Dictionary<string, OptionDefinition>(StringComparer.OrdinalIgnoreCase);
            foreach (OptionDefinition def in optionDefinitions)
            {
                foreach (string alias in def.Aliases)
                {
                    lookup[alias] = def;
                }
            }

            // Process the command-line arguments.
            int index = 0;
            while (index < args.Length)
            {
                string arg = args[index];
                if (lookup.TryGetValue(arg, out OptionDefinition? optionDef))
                {
                    // Make sure there are enough remaining parameters, if needed.
                    if (index + optionDef.ParameterCount >= args.Length)
                    {
                        throw new ArgumentException(
                            $"Option '{arg}' requires {optionDef.ParameterCount} parameter(s).");
                    }

                    // Collect the parameters for the option.
                    string[] parameters = new string[optionDef.ParameterCount];
                    for (int j = 0; j < optionDef.ParameterCount; j++)
                    {
                        parameters[j] = args[index + j + 1];
                    }

                    // Execute the option's action.
                    optionDef.Apply(options, parameters);

                    // Skip over the option and its parameters.
                    index += optionDef.ParameterCount + 1;
                }
                else
                {
                    // Not recognized as an option -- treat as a positional argument.
                    options.PositionalArguments.Add(arg);
                    index++;
                }
            }

            return options;
        }
    }
}
