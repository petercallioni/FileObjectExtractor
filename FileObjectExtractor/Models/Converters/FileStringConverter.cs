using Avalonia.Data.Converters;
using System;
using System.Globalization;
using System.IO;

namespace FileObjectExtractor.Models.Converters
{
    public class FileStringConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            Uri? uri = value as Uri;

            if (uri != null && uri.IsAbsoluteUri)
            {
                FileInfo fileInfo = new FileInfo(uri.AbsolutePath);

                // Is a file, truncate it
                if (fileInfo.Exists)
                {
                    return fileInfo.Name;
                }
                else
                {
                    return value;
                }
            }

            return Constants.StringConstants.DropDefaultText;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
