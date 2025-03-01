using Avalonia.Data.Converters;
using System;
using System.Globalization;

namespace FileObjectExtractor.Models.Converters
{
    public class FileNameShortener : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            string? fileName = value as string;

            if (fileName != null)
            {
                if (fileName.Length > 50)
                {
                    return fileName.Substring(0, 50) + "...";
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
