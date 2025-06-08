using FileObjectExtractor.ViewModels;
using System.Collections.Generic;

namespace FileObjectExtractor.Models
{
    public class NagivationState
    {
        readonly List<ExtractedFileViewModel>? extractedFiles;
        readonly InputFileViewModel? inputFile;
        public NagivationState(List<ExtractedFileViewModel>? extractedFiles, InputFileViewModel? inputFile)
        {
            this.extractedFiles = extractedFiles;
            this.inputFile = inputFile;
        }

        public List<ExtractedFileViewModel>? ExtractedFiles => extractedFiles;

        public InputFileViewModel? InputFile => inputFile;
    }
}
