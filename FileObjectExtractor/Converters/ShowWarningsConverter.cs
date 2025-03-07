using Avalonia.Data.Converters;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace FileObjectExtractor.Converters
{
    public class ShowWarningsConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            List<string>? warnings = value as List<string>;

            if (warnings != null)
            {
                if (warnings.Count > 0)
                {
                    return true;
                }
            }

            return false;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
