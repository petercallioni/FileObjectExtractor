using Avalonia.Data.Converters;
using Avalonia.Media;
using System;
using System.Globalization;

namespace FileObjectExtractor.Converters
{
    public class ProgressTextColourConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            double current = (double)(value ?? 0.0);
            double maximum = (double)(parameter ?? 1.0);

            if (current >= (maximum / 2d)) // If the progress is over 50%
            {
                return Brush.Parse("#FFFFFF");
            }

            return Brush.Parse("#000000");
        }
        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}