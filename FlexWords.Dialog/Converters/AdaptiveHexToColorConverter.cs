using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using FlexWords.Dialog.Extensions;

namespace FlexWords.Dialog.Converters
{
    public class AdaptiveHexToColorConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values is null || values.Length != 2) return DependencyProperty.UnsetValue;

            if (values[0] is string hexValue && values[1] is double brightness)
            {
                return hexValue.ToColor().AdaptiveAdjustBrightness(brightness);
            }

            return DependencyProperty.UnsetValue;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
