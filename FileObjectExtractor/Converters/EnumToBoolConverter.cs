using Avalonia.Data;
using Avalonia.Data.Converters;
using System;
using System.Globalization;

namespace FileObjectExtractor.Converters
{
    public class EnumToBoolConverter : IValueConverter
    {
        public bool Negate { get; set; } // Add a property to control inversion

        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value == null || parameter == null)
                return false;

            bool result = value.ToString()!.Equals(parameter.ToString(), StringComparison.OrdinalIgnoreCase);
            return Negate ? !result : result; // Apply negation if needed
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is bool booleanValue && booleanValue)
            {
                return Enum.Parse(targetType, parameter!.ToString()!);
            }
            return BindingOperations.DoNothing;
        }
    }
}