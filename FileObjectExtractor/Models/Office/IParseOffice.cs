using System;
using System.Collections.Generic;

namespace FileObjectExtractor.Models.Office
{
    public interface IParseOffice
    {
        public List<ExtractedFile> GetExtractedFiles(Uri filePath);
        public OfficeType OfficeType { get; init; }
    }
}
