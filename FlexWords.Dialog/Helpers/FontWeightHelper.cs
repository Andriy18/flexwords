using System;
using System.Linq;
using System.Windows;

namespace FlexWords.Dialog.Helpers
{
    public static class FontWeightHelper
    {
        private static readonly FontWeight[] _all =
        {
            FontWeights.Thin,
            FontWeights.ExtraLight,
            FontWeights.Light,
            FontWeights.Normal, // 400
            FontWeights.Medium,
            FontWeights.DemiBold,
            FontWeights.Bold,
            FontWeights.ExtraBold,
            FontWeights.Black,
            FontWeights.ExtraBlack,
        };

        public static readonly string[] FontWeightNames =
            _all.Select(i => i.ToString()).ToArray();

        public static FontWeight GetFontWeight(string fontWeightName)
        {
            return GetFontWeight(GetIndex(fontWeightName));
        }

        public static FontWeight GetFontWeight(int index)
        {
            if (0 <= index && index < _all.Length)
            {
                return _all[index];
            }

            return _all[3];
        }

        public static int GetIndex(string fontWeightName)
        {
            return Array.IndexOf(FontWeightNames, fontWeightName);
        }

        public static int GetIndex(FontWeight fontWeight)
        {
            return Array.IndexOf(_all, fontWeight);
        }
    }
}
