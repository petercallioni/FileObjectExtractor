using FileObjectExtractor.Interfaces;
using System;
using System.IO;

namespace FileObjectExtractor.Models
{
    public static class OfficeParserPicker
    {
        public static IParseOffice GetOfficeParser(Uri fileName)
        {
            FileInfo file = new FileInfo(fileName.AbsolutePath);

            switch (file.Extension.ToUpper())
            {
                case (".DOCX"):
                case (".DOCM"):
                case (".DOTM"):
                case (".DOTX"):
                    return new ParseOfficeXmlWord();
                case (".XLSX"):
                case (".XLXM"):
                case (".XLTM"):
                case (".XLTX"):
                case (".XLW"):
                    return new ParseOfficeXmlExcel();
                case (".PPTX"):
                case (".POTX"):
                case (".PPTM"):
                case (".PPSM"):
                case (".PPSX"):
                    return new ParseOfficeXmlPowerpoint();
            }

            throw new NotImplementedException("Parsing specified file type not implemented.");
        }
    }
}
