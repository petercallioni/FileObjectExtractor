using FileObjectExtractor.Interfaces;
using System;
using System.IO;

namespace FileObjectExtractor.Models
{
    public static class OfficeParserPicker
    {
        public static IParseOffice GetOfficeParser(string fileName)
        {
            FileInfo file = new FileInfo(fileName);

            switch (file.Extension.ToUpper())
            {
                case (".DOCX"):
                    return new ParseOfficeXmlWord();
                case (".XLSX"):
                    return new ParseOfficeXmlExcel();
                case (".PPTX"):
                    return new ParseOfficeXmlPowerpoint();
            }

            throw new NotImplementedException("Parsing specified file type not implemented.");
        }
    }
}
