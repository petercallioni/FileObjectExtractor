using FileObjectExtractor.Extensions;
using System;
using System.IO;
using System.Linq;

namespace FileObjectExtractor.Models.Office
{

    public static class OfficeParserPicker
    {
        public static string[] WORD_EXTENSIONS = { ".DOCX", ".DOCM", ".DOTM", ".DOTX" };
        public static string[] EXCEL_EXTENSIONS = { ".XLSX", ".XLXM", ".XLTM", ".XLTX", ".XLW" };
        public static string[] POWERPOINT_EXTENSIONS = { ".PPTX", ".POTX", ".PPTM", ".PPSM", ".PPSX" };
        public static string[] ALL_EXTENSIONS =
            WORD_EXTENSIONS
            .Concat(EXCEL_EXTENSIONS)
            .Concat(POWERPOINT_EXTENSIONS)
            .ToArray();

        public static bool IsOfficeFile(string fileName)
        {
            FileInfo file = new FileInfo(fileName);
            return ALL_EXTENSIONS.Contains(file.Extension.ToUpper());
        }

        public static IParseOffice GetOfficeParser(Uri fileName)
        {
            FileInfo file = new FileInfo(fileName.UnescapedString());
            return GetOfficeParser(file);
        }

        public static IParseOffice GetOfficeParser(string fileName)
        {
            FileInfo file = new FileInfo(fileName);
            return GetOfficeParser(file);
        }

        public static IParseOffice GetOfficeParser(FileInfo file)
        {
            if (WORD_EXTENSIONS.Contains(file.Extension.ToUpper()))
            {
                return new ParseOfficeXmlWord();
            }
            else if (EXCEL_EXTENSIONS.Contains(file.Extension.ToUpper()))
            {
                return new ParseOfficeXmlExcel();
            }
            else if (POWERPOINT_EXTENSIONS.Contains(file.Extension.ToUpper()))
            {
                return new ParseOfficeXmlPowerpoint();
            }

            throw new NotImplementedException("This type of file can not be used");
        }
    }
}
