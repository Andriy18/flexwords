using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows;
using FlexWords.Dialog.Extensions;

namespace FlexWords.Dialog.Converters
{
    public class HexToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string hexValue)
            {
                return hexValue.ToColor();
            }

            return DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
