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
    }
}
