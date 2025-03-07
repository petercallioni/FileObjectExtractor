using Avalonia.Data.Converters;
using System;
using System.Globalization;

namespace FileObjectExtractor.Converters
{
    public class ByteSizeConverter : IValueConverter
    {
        private static readonly string[] SizeSuffixes = { "B", "KiB", "MiB", "GiB", "TiB" };
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is byte[] byteArray)
            {
                double size = byteArray.Length;
                int order = 0;

                while (size >= 1024 && order < SizeSuffixes.Length - 1)
                {
                    order++;
                    size /= 1024;
                }

                return string.Format("{0:0.##} {1}", size, SizeSuffixes[order]);
            }

            return "0 B";
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            Avalonia.Data.BindingNotification state = new Avalonia.Data.BindingNotification(value);
            state.Error = new NotImplementedException();
            state.ErrorType = Avalonia.Data.BindingErrorType.Error;
            return state;
        }
    }
}