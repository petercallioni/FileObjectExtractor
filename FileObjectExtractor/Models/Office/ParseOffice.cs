using System;
using System.Collections.Generic;

namespace FileObjectExtractor.Models.Office
{
    public abstract class ParseOffice : IParseOffice
    {
        public abstract List<ExtractedFile> GetExtractedFiles(Uri filePath);
        public OfficeType OfficeType { get; init; } = OfficeType.UNKNOWN;
    }
}
