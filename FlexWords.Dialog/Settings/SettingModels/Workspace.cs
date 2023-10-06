namespace FlexWords.Dialog
{
    public sealed class Workspace
    {
        public string Background { get; set; } = string.Empty;
        public string Foreground { get; set; } = string.Empty;
        public string SelectedForeground { get; set; } = string.Empty;
        public double HoveredTextTransparency { get; set; }
        public double InactiveTextTransparency { get; set; }

        public Workspace Clone()
        {
            return new Workspace
            {
                Background = Background,
                Foreground = Foreground,
                SelectedForeground = SelectedForeground,
                HoveredTextTransparency = HoveredTextTransparency,
                InactiveTextTransparency = InactiveTextTransparency,
            };
        }


        public readonly static Workspace Default = new()
        {
            Background = "#202124",
            Foreground = "#E5E5E5",
            SelectedForeground = "#F4D03F",
            HoveredTextTransparency = 0.45,
            InactiveTextTransparency = 0.05,
        };
    }
}