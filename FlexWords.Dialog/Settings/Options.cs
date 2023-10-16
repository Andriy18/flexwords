using System;
using System.Collections.Generic;
using FlexWords.Entities.Structs;
using FlexWords.Dialog.Settings;
using Newtonsoft.Json;

namespace FlexWords.Dialog
{
    public static partial class Options
    {
        public static Bookmark LastBkmark
        {
            get => Bookmark.Parse(GeneralOptions.Default.LastBkmark);            
            set
            {
                GeneralOptions.Default.LastBkmark = value.ToString();
                GeneralOptions.Default.Save();
            }
        }

        public static Bookmark? CaptureBkmark
        {
            get
            {
                if (string.IsNullOrEmpty(GeneralOptions.Default.CaptureBkmark)) return null;

                return Bookmark.Parse(GeneralOptions.Default.CaptureBkmark);
            }
            set
            {
                GeneralOptions.Default.CaptureBkmark = value?.ToString() ?? null;
                GeneralOptions.Default.Save();
            }
        }

        #region General

        public static bool LogoPreview
        {
            get => GeneralOptions.Default.LogoPreview;
            set
            {
                GeneralOptions.Default.LogoPreview = value;
                GeneralOptions.Default.Save();
            }
        }

        public static bool AdaptiveWordSpacing
        {
            get => GeneralOptions.Default.AdaptiveWordSpacing;
            set
            {
                GeneralOptions.Default.AdaptiveWordSpacing = value;
                GeneralOptions.Default.Save();
            }
        }

        public static bool AdaptiveKerning
        {
            get => GeneralOptions.Default.AdaptiveKerning;
            set
            {
                GeneralOptions.Default.AdaptiveKerning = value;
                GeneralOptions.Default.Save();
            }
        }

        public static bool AutoCopySelectedText
        {
            get => GeneralOptions.Default.AutoCopySelectedText;
            set
            {
                GeneralOptions.Default.AutoCopySelectedText = value;
                GeneralOptions.Default.Save();
            }
        }

        #endregion

        #region Translator

        public static bool UseTranslator
        {
            get => GeneralOptions.Default.UseTranslator;
            set
            {
                GeneralOptions.Default.UseTranslator = value;
                GeneralOptions.Default.Save();
            }
        }

        public static bool UseGoogleTranslate
        {
            get => GeneralOptions.Default.UseGoogleTranslate;
            set
            {
                GeneralOptions.Default.UseGoogleTranslate = value;
                GeneralOptions.Default.Save();
            }
        }

        public static bool UseSpellChecker
        {
            get => GeneralOptions.Default.UseSpellChecker;
            set
            {
                GeneralOptions.Default.UseSpellChecker = value;
                GeneralOptions.Default.Save();
            }
        }

        public static bool AddPronunciation
        {
            get => GeneralOptions.Default.AddPronunciation;
            set
            {
                GeneralOptions.Default.AddPronunciation = value;
                GeneralOptions.Default.Save();
            }
        }

        public static bool ShowSynonyms
        {
            get => GeneralOptions.Default.ShowSynonyms;
            set
            {
                GeneralOptions.Default.ShowSynonyms = value;
                GeneralOptions.Default.Save();
            }
        }

        #endregion

        #region options 1

        public static double FontSize
        {
            get => GeneralOptions.Default.FontSize;
            set
            {
                GeneralOptions.Default.FontSize = value;
                GeneralOptions.Default.Save();
            }
        }

        public static int FontFamily
        {
            get => GeneralOptions.Default.FontFamily;
            set
            {
                GeneralOptions.Default.FontFamily = value;
                GeneralOptions.Default.Save();
            }
        }

        public static int FontWeight
        {
            get => GeneralOptions.Default.FontWeight;
            set
            {
                GeneralOptions.Default.FontWeight = value;
                GeneralOptions.Default.Save();
            }
        }

        #endregion

        #region options 2

        public static double VerticalOffset
        {
            get => GeneralOptions.Default.VerticalOffset;
            set
            {
                GeneralOptions.Default.VerticalOffset = value;
                GeneralOptions.Default.Save();
            }
        }

        public static double HorizontalOffset
        {
            get => GeneralOptions.Default.HorizontalOffset;
            set
            {
                GeneralOptions.Default.HorizontalOffset = value;
                GeneralOptions.Default.Save();
            }
        }

        public static double AreaWidth
        {
            get => GeneralOptions.Default.AreaWidth;
            set
            {
                GeneralOptions.Default.AreaWidth = value;
                GeneralOptions.Default.Save();
            }
        }

        #endregion

        #region options 3

        public static double Kerning
        {
            get => GeneralOptions.Default.Kerning;
            set
            {
                GeneralOptions.Default.Kerning = value;
                GeneralOptions.Default.Save();
            }
        }

        public static double WordSpacing
        {
            get => GeneralOptions.Default.WordSpacing;
            set
            {
                GeneralOptions.Default.WordSpacing = value;
                GeneralOptions.Default.Save();
            }
        }

        public static double LineSpacing
        {
            get => GeneralOptions.Default.LineSpacing;
            set
            {
                GeneralOptions.Default.LineSpacing = value;
                GeneralOptions.Default.Save();
            }
        }

        public static double Indent
        {
            get => GeneralOptions.Default.Indent;
            set
            {
                GeneralOptions.Default.Indent = value;
                GeneralOptions.Default.Save();
            }
        }

        #endregion

        public static string LastUsedBook
        {
            get => GeneralOptions.Default.LastUsedBook;
            set
            {
                GeneralOptions.Default.LastUsedBook = value;
                GeneralOptions.Default.Save();
            }
        }

        public static string LastUsedFolder
        {
            get => GeneralOptions.Default.LastUsedFolder;
            set
            {
                GeneralOptions.Default.LastUsedFolder = value;
                GeneralOptions.Default.Save();
            }
        }

        public static int ThemeIndex
        {
            get => GeneralOptions.Default.ThemeIndex;
            set
            {
                value = Math.Clamp(value, 0, int.MaxValue);

                GeneralOptions.Default.ThemeIndex = value;
                GeneralOptions.Default.Save();
            }
        }

        public static ThemeSet[] Themes
        {
            get
            {
                var themes = new List<ThemeSet>();
                string data = GeneralOptions.Default.Themes;

                try
                {
                    var workspaces = JsonConvert.DeserializeObject<ThemeSet[]>(data);

                    if (workspaces is not null && workspaces.Length > 0)
                    {
                        themes.AddRange(workspaces);
                    }
                }
                catch
                {
                    data = JsonConvert.SerializeObject(Array.Empty<ThemeSet>());
                    GeneralOptions.Default.Themes = data;
                    GeneralOptions.Default.Save();
                }

                return themes.ToArray();
            }
        }
    }

    public static partial class Options
    {
        public static ThemeSet GetTheme()
        {
            ThemeSet[] themes = Themes;

            if (themes.Length is 0)
            {
                ThemeIndex = 0;
                themes = AddTheme();

                return themes[ThemeIndex];
            }

            if (themes.Length <= ThemeIndex)
            {
                ThemeIndex = themes.Length - 1;

                return themes[ThemeIndex];
            }

            return themes[ThemeIndex];
        }

        public static ThemeSet[] AddTheme()
        {
            var themes = new List<ThemeSet>(Themes) { ThemeSet.Default };
            string data = JsonConvert.SerializeObject(themes.ToArray());

            GeneralOptions.Default.Themes = data;
            GeneralOptions.Default.Save();

            return themes.ToArray();
        }

        public static ThemeSet[] DeleteTheme()
        {
            var themes = new List<ThemeSet>(Themes);

            if (themes.Count is 0 || themes.Count <= ThemeIndex)
            {
                return themes.ToArray();
            }

            themes.Remove(themes[ThemeIndex]);
            string data = JsonConvert.SerializeObject(themes.ToArray());

            GeneralOptions.Default.Themes = data;
            GeneralOptions.Default.Save();

            return themes.ToArray();
        }

        public static void UpdateTheme(ThemeSet theme)
        {
            var themes = new List<ThemeSet>(Themes);

            if (themes.Count is 0 || themes.Count <= ThemeIndex) return;

            themes[ThemeIndex] = theme;
            string data = JsonConvert.SerializeObject(themes.ToArray());

            GeneralOptions.Default.Themes = data;
            GeneralOptions.Default.Save();
        }
    }
}
