using Avalonia.Data.Converters;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace FileObjectExtractor.Models.Converters
{
    public class DisplayWarningsConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            List<string>? warnings = value as List<string>;
            StringBuilder warningBuilder = new StringBuilder();

            if (warnings != null)
            {
                if (warnings.Count > 0)
                {
                    warningBuilder.Append("Caution:\n");
                    warningBuilder.AppendJoin("\n", warnings);
                }
            }

            return warningBuilder.ToString();
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}