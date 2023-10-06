namespace FlexWords.Translator
{
    public sealed class Synonym
    {
        public string? Text { get; set; }
        public string? Type { get; set; }

        public override string? ToString()
        {
            if (Type is "Unknown")
            {
                return Text;
            }

            return string.Format("{0} ({1})", Text, Type);
        }
    }
}
