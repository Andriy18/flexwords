using System;
using System.Windows.Media;

namespace FlexWords.Dialog.Extensions
{
    public static class ColorExtensions
    {
        public static Brush ToBrush(this string hexadecimal)
        {
            return new SolidColorBrush(hexadecimal.ToColor());
        }

        public static Color ToColor(this string hexadecimal)
        {
            return (Color)ColorConverter.ConvertFromString(hexadecimal);
        }

        public static Color ToColor(this Brush brush)
        {
            if (brush is SolidColorBrush colorBrush)
            {
                return colorBrush.Color;
            }

            throw new Exception();
        }

        public static string ToHex(this Brush brush)
        {
            return brush.ToColor().ToHex();
        }

        public static Color GetReverse(this Color original)
        {
            return Color.FromRgb(
                (byte)(255 - original.R),
                (byte)(255 - original.G),
                (byte)(255 - original.B));
        }

        public static Brush GetBrushReverse(this Color original)
        {
            return new SolidColorBrush(original.GetReverse());
        }

        public static string ToHex(this Color color)
        {
            return "#" + ColorToHexString(color);
        }

        public static Color MixWith(this Color source, Color color)
        {
            byte r = (byte)((source.R + color.R) / 2);
            byte g = (byte)((source.G + color.G) / 2);
            byte b = (byte)((source.B + color.B) / 2);

            return Color.FromRgb(r, g, b);
        }

        private static string ColorToHexString(Color color)
        {
            byte[] byteArray = { color.A, color.R, color.G, color.B };

            string alphaString = color.A != byte.MaxValue
                ? BitConverter.ToString(byteArray, 0, 1)
                : string.Empty;
            string rgbString = BitConverter.ToString(byteArray, 1).Replace("-", "");

            return string.Format("{0}{1}", alphaString, rgbString);
        }

        public static Brush AdjustBrightness(this Brush brush, double factor)
        {
            return new SolidColorBrush(brush.ToColor().AdjustBrightness(factor));
        }

        public static Color AdjustBrightness(this Color color, double factor)
        {
            factor = Math.Clamp(factor, -1, 1);

            byte r = (byte)Math.Max(0, Math.Min(255, color.R + 255 * factor));
            byte g = (byte)Math.Max(0, Math.Min(255, color.G + 255 * factor));
            byte b = (byte)Math.Max(0, Math.Min(255, color.B + 255 * factor));

            return Color.FromRgb(r, g, b);
        }

        public static bool IsColorDark(this Color color)
        {
            double luminance = 0.299 * color.R + 0.587 * color.G + 0.114 * color.B;

            return luminance < 128;
        }

        public static Color AdaptiveAdjustBrightness(this Color color, double factor)
        {
            factor = Math.Clamp(factor, 0, 1);

            if (color.IsColorDark())
            {
                return color.AdjustBrightness(factor);
            }

            return color.AdjustBrightness(-factor);
        }

        public static Brush AdaptiveAdjustBrightness(this Brush brush, double factor)
        {
            factor = Math.Clamp(factor, 0, 1);

            if (brush.ToColor().IsColorDark())
            {
                return brush.AdjustBrightness(factor);
            }

            return brush.AdjustBrightness(-factor);
        }
    }
}
