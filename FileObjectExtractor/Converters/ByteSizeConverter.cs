using Avalonia.Data.Converters;
using System;
using System.Globalization;

namespace FileObjectExtractor.Converters
{
    public class ByteSizeConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is byte[] byteArray)
            {
                return ByteSizeFormatter.Format(byteArray);
            }
            else if (value is long byteCount)
            {
                return ByteSizeFormatter.Format(byteCount);
            }
            else if (value is int intValue)
            {
                return ByteSizeFormatter.Format(intValue);
            }

            return "0 B";
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            // Typically unused; can throw or return a BindingNotification if needed.
            throw new NotImplementedException();
        }
    }
}