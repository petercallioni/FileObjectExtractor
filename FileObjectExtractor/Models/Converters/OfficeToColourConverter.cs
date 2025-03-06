using Avalonia.Data.Converters;
using Avalonia.Media;
using System;
using System.Globalization;

namespace FileObjectExtractor.Models.Converters
{
    public class OfficeToColourConverter : IValueConverter
    {
        public Object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            OfficeType officeType = (OfficeType)(value ?? OfficeType.UNKNOWN);

            switch (officeType)
            {
                case OfficeType.WORD:
                    return SolidColorBrush.Parse("#108EF4");
                case OfficeType.EXCEL:
                    return SolidColorBrush.Parse("#3DC33A");
                case OfficeType.POWERPOINT:
                    return SolidColorBrush.Parse("#D98B23");
                default:
                    return SolidColorBrush.Parse("#282828");
            }
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return new NotImplementedException();
        }
    }
}
