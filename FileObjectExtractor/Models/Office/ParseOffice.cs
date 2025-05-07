using FileObjectExtractor.Extensions;
using System;
using System.Collections.Generic;
using System.IO;

namespace FileObjectExtractor.Models.Office
{
    public abstract class ParseOffice : IParseOffice
    {
        public abstract List<ExtractedFile> GetExtractedFiles(Uri filePath);
        public OfficeType OfficeType { get; init; } = OfficeType.UNKNOWN;

        protected byte[] OpenOfficeFile(Uri filepath)
        {
            byte[] fileBytes;
            using (FileStream stream = new FileStream(
                filepath.UnescapedString(),
                FileMode.Open,
                FileAccess.Read,
                FileShare.ReadWrite))
            {
                fileBytes = new byte[stream.Length];
                stream.Read(fileBytes, 0, fileBytes.Length);
            }
            ;

            return fileBytes;
        }
    }
}
