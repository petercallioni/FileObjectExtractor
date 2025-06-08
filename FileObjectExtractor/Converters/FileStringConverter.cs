using Avalonia.Data.Converters;
using FileObjectExtractor.Extensions;
using System;
using System.Globalization;
using System.IO;

namespace FileObjectExtractor.Converters
{
    public class FileStringConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            Uri? uri = value as Uri;

            if (uri != null && uri.IsAbsoluteUri)
            {
                FileInfo fileInfo = new FileInfo(uri.UnescapedString());
                return fileInfo.Name;
            }

            return Constants.StringConstants.DEFAULT_DROP_TEXT;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
