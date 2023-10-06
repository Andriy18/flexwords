namespace FlexWords.Translator.Interfaces
{
    internal interface ITranslationHandler
    {
        Task<string> TranslateAsync(string text, bool checkSpelling = false);
    }
}
