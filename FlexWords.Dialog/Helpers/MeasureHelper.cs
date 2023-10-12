using System;
using System.Globalization;
using System.Windows.Media;
using System.Windows;

namespace FlexWords.Dialog.Helpers
{
    public static class MeasureHelper
    {
        private static FormattedText? _formatted;
        private static readonly CultureInfo _cultureInfo = CultureInfo.CurrentCulture;
        private static readonly SolidColorBrush _brush = Brushes.Black;
        private static readonly NumberSubstitution _numberSubstitution = new();
        private static readonly Typeface _typeface = new(
            new FontFamily("Verdana"), FontStyles.Normal, FontWeights.Bold, FontStretches.Normal);

        public static double GetFontHeight(double fontSize, string fontFamily)
        {
            FontFamily family = FontFamilyHelper.GetFontFamily(fontFamily);

            return Math.Ceiling(fontSize * family.LineSpacing);
        }

        public static double GetBestFontSize(string text, double maxWidth, double maxHeight, double scale = 1.0)
        {
            if (Math.Abs(scale - 1.0) > double.Epsilon)
            {
                maxWidth *= scale;
                maxHeight *= scale;
            }

            double desireSize = (int)(maxHeight / _typeface.FontFamily.LineSpacing);
            _formatted = new FormattedText(text, _cultureInfo, 0, _typeface, desireSize, _brush, _numberSubstitution, 1);

            if (maxWidth < _formatted.Width)
            {
                desireSize = Math.Ceiling(desireSize / (_formatted.Width / maxWidth));
                _formatted.SetFontSize(desireSize);

                while (maxWidth < _formatted.Width)
                {
                    desireSize--;

                    _formatted.SetFontSize(desireSize);
                }

                return desireSize;
            }
            else return desireSize;
        }

        public static Thickness GetMargin()
        {
            double right = -Options.HorizontalOffset;
            double left = Options.HorizontalOffset;
            double top = -Options.VerticalOffset;
            double bottom = Options.VerticalOffset;

            return new Thickness(left, top, right, bottom);
        }
    }
}
