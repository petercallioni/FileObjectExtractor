using FileObjectExtractor.Models;
using System.Collections.Generic;

namespace FileObjectExtractor.Interfaces
{
    public interface IParseOffice
    {
        public List<ExtractedFile> GetExtractedFiles(string filePath);
    }
}
