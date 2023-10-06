using System;
using System.Globalization;
using System.Windows.Media;
using System.Windows;
using System.IO;
using System.Windows.Media.Imaging;

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
            double right = -Options.HorizontalOffset + Options.LeftRightOffset;
            double left = Options.HorizontalOffset + Options.LeftRightOffset;
            double top = -Options.VerticalOffset;
            double bottom = Options.VerticalOffset;

            return new Thickness(left, top, right, bottom);
        }

        public static BitmapImage ToBitmapImage(RenderTargetBitmap render, int width, int height)
        {
            var bitmapImage = new BitmapImage();
            using (var stream = new MemoryStream())
            {
                var encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(render));
                encoder.Save(stream);
                stream.Seek(0, SeekOrigin.Begin);

                bitmapImage.BeginInit();
                bitmapImage.DecodePixelWidth = width;
                bitmapImage.DecodePixelHeight = height;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.StreamSource = stream;
                bitmapImage.EndInit();
            }
            return bitmapImage;
        }
    }
}
