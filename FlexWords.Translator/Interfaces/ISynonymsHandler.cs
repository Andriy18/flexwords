namespace FlexWords.Translator.Interfaces
{
    internal interface ISynonymsHandler
    {
        Task<IEnumerable<Synonym>> GetSynonymsAsync(string text, bool checkSpelling = false);
    }
}
