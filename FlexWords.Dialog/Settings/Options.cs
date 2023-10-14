using System.Windows;
using FlexWords.Entities.Structs;
using Newtonsoft.Json;
using FlexWords.Dialog.Helpers;
using FlexWords.Dialog.Settings;

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

        public static Workspace Workspace1
        {
            get => GetWorkspace(0);
            set => SetWorkspace(0, value);
        }

        public static Workspace Workspace2
        {
            get => GetWorkspace(1);
            set => SetWorkspace(1, value);
        }

        public static int LastWorkspaceIndex
        {
            get => GeneralOptions.Default.LastWorkspaceIndex;
            set
            {
                GeneralOptions.Default.LastWorkspaceIndex = value;
                GeneralOptions.Default.Save();
            }
        }

        public static bool SetDefaultValues
        {
            get => GeneralOptions.Default.SetDefaultValues;
            set
            {
                GeneralOptions.Default.SetDefaultValues = value;
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
            get => GeneralOptions.Default.LastBookPath;
            set
            {
                GeneralOptions.Default.LastBookPath = value;
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
    }

    public static partial class Options
    {
        // helping code is here

        private static Workspace GetWorkspace(int index)
        {
            string data = GeneralOptions.Default.Workspaces;

            if (string.IsNullOrEmpty(data) ||
                JsonConvert.DeserializeObject<Workspace[]>(data) is not Workspace[] workspaces)
            {
               workspaces = new Workspace[2]
               {
                    Workspace.Default,
                    Workspace.Default
               };

                data = JsonConvert.SerializeObject(workspaces);

                GeneralOptions.Default.Workspaces = data;
                GeneralOptions.Default.Save();
            }

            return workspaces[index];
        }

        private static void SetWorkspace(int index, Workspace workspace)
        {
            if (index == 0)
            {
                var workspaces = new Workspace[2]
                {
                    workspace,
                    GetWorkspace(1),
                };

                GeneralOptions.Default.Workspaces = JsonConvert.SerializeObject(workspaces);
                GeneralOptions.Default.Save();
            }
            else if (index == 1)
            {
                var workspaces = new Workspace[2]
                {
                    GetWorkspace(0),
                    workspace,
                };

                GeneralOptions.Default.Workspaces = JsonConvert.SerializeObject(workspaces);
                GeneralOptions.Default.Save();
            }
        }

        public static void SetDefault1()
        {
            FontSize = 21;
            FontFamily = FontFamilyHelper.GetIndex("Verdana");
            FontWeight = FontWeightHelper.GetIndex(FontWeights.Normal);
        }

        public static void SetDefault2()
        {
            HorizontalOffset = 0;
            VerticalOffset = 0;
            AreaWidth = 600;
        }

        public static void SetDefault3()
        {
            Indent = 45;
            Kerning = 4;
            WordSpacing = 15;
            LineSpacing = 8;
        }
    }
}
