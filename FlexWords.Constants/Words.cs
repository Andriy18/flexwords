namespace FlexWords.Constants
{
    public static class Words
    {
        public const string ABCLower = "abcdefghijklmnopqrstuvwxyz";
        public const string ABCUpper = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        public const string Figures = "0123456789";
        public const string Apostrophes = "\'`‘’";
        public const string Dashes = "-–"; //8211
        public const string WhiteSpace = " ";
        public const string Dot = ".";
        public const string SentenceEnd = "?!;";
        public const string Dialogue = "\"“”";
        public const string BaseSymbols = ",:()[]/\\|#&*^%$@=+";
        public const string LettersCheck = ABCLower + ABCUpper + Figures;
        public const string DictionaryCheck = LettersCheck + "\'\"-. " + SentenceEnd + BaseSymbols;

        public const string DefaultChapter = "1";
        public const string ChapterSeparator = "###";

        public static class FileFormat
        {
            public const string TxtExt = ".txt";
            public const string EpubExt = ".epub";
            public const string PdfExt = ".pdf";
            public const string DocxExt = ".docx";

            public static readonly string[] SupportedFormats =
            {
                TxtExt,
                EpubExt,

                // No file formats are currently supported
                //PdfExt,
                //DocxExt
            };

            public static readonly Dictionary<string, string> FormatThemes = new()
            {
                { TxtExt, "#ADADAD" },
                { EpubExt, "#8ABA20" },
                { PdfExt, "#F15B48" },
                { DocxExt, "#185ABD" },
                { "Driver", "#505254" },
                { "Folder", "#C4992D" },
            };
        }
    }
}
