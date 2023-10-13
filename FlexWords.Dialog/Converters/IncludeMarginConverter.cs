using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows;

namespace FlexWords.Dialog.Converters
{
    public class IncludeMarginConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values is null || values.Length != 2) return DependencyProperty.UnsetValue;

            if (values[0] is double doubleValue && values[1] is Thickness margin)
            {
                return doubleValue - (margin.Left + margin.Right);
            }

            return DependencyProperty.UnsetValue;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
