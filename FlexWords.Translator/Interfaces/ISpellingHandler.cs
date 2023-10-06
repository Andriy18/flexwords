namespace FlexWords.Translator.Interfaces
{
    internal interface ISpellingHandler
    {
        Task<string> SpellingCheckAsync(string text);
    }
}
