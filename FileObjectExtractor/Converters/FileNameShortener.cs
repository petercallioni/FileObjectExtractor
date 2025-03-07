using Avalonia.Data.Converters;
using FileObjectExtractor.Constants;
using System;
using System.Globalization;

namespace FileObjectExtractor.Converters
{
    public class FileNameShortener : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            string? fileName = value as string;

            if (fileName != null)
            {
                if (fileName.Length > IntContstants.MAX_FILE_NAME_CHARS)
                {
                    return fileName.Substring(0, IntContstants.MAX_FILE_NAME_CHARS) + "...";
                }
                else
                {
                    return fileName;
                }
            }
            return value;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
