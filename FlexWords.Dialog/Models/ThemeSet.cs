namespace FlexWords.Dialog
{
    public sealed class ThemeSet
    {
        public string Foreground { get; set; } = string.Empty;
        public string SelectedForeground { get; set; } = string.Empty;
        public double HoveredTextTransparency { get; set; }
        public double InactiveTextTransparency { get; set; }
        public string Background { get; set; } = string.Empty;
        public double CoverOpacity { get; set; }
        public int CoverIndex { get; set; }


        public readonly static ThemeSet Default = new()
        {
            Background = "#202124",
            Foreground = "#E5E5E5",
            SelectedForeground = "#F4D03F",
            HoveredTextTransparency = 0.45,
            InactiveTextTransparency = 0.05,
            CoverOpacity = 1.0,
            CoverIndex = 0
        };
    }
}