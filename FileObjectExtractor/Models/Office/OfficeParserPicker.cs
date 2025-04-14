using FileObjectExtractor.Extensions;
using System;
using System.IO;

namespace FileObjectExtractor.Models.Office
{
    public static class OfficeParserPicker
    {
        public static IParseOffice GetOfficeParser(Uri fileName)
        {
            FileInfo file = new FileInfo(fileName.UnescapedString());

            switch (file.Extension.ToUpper())
            {
                case ".DOCX":
                case ".DOCM":
                case ".DOTM":
                case ".DOTX":
                    return new ParseOfficeXmlWord();
                case ".XLSX":
                case ".XLXM":
                case ".XLTM":
                case ".XLTX":
                case ".XLW":
                    return new ParseOfficeXmlExcel();
                case ".PPTX":
                case ".POTX":
                case ".PPTM":
                case ".PPSM":
                case ".PPSX":
                    return new ParseOfficeXmlPowerpoint();
            }

            throw new NotImplementedException("This type of file can not be used");
        }
    }
}
