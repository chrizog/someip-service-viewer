using Avalonia.Data.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceViewer.Converter
{
    public class HexConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, System.Globalization.CultureInfo culture)
        {
            if (value is int intValue)
            {
                return "0x" + intValue.ToString("X4");
            }
            else if (value is uint uintValue)
            {
                return "0x" + uintValue.ToString("X4");
            }
            else if (value is short shortValue)
            {
                   return "0x" + shortValue.ToString("X4");
            }
            else if (value is ushort ushortValue)
            {
                return "0x" + ushortValue.ToString("X4");
            }
            else if (value is byte byteValue)
            {
                return "0x" + byteValue.ToString("X2");
            }
            return string.Empty;
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, System.Globalization.CultureInfo culture)
        {
            // not needed
            return value;
        }
    }
}
