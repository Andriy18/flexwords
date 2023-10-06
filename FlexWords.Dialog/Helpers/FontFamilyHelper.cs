using System;
using System.Collections.Immutable;
using System.Linq;
using System.Windows.Media;

namespace FlexWords.Dialog.Helpers
{
    public static class FontFamilyHelper
    {
        private readonly static ImmutableArray<FontFamily> _all =
            Fonts.SystemFontFamilies.ToImmutableArray();

        public readonly static string[] FontFamilyNames =
            _all.Select(i => i.Source).ToArray();

        public static int GetIndex(FontFamily fontFamily)
        {
            return _all.IndexOf(fontFamily);
        }

        public static int GetIndex(string fontFamilyName)
        {
            return Array.IndexOf(FontFamilyNames, fontFamilyName);
        }

        public static FontFamily GetFontFamily(string fontFamilyName)
        {
            return GetFontFamily(GetIndex(fontFamilyName));
        }

        public static FontFamily GetFontFamily(int index)
        {
            if (0 <= index && index < _all.Length)
            {
                return _all[index];
            }
            
            return _all[Array.IndexOf(FontFamilyNames, "Segoe UI")];
        }
    }
}
