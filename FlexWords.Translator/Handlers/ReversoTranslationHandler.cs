using FlexWords.Translator.Interfaces;
using ReversoAPI;

namespace FlexWords.Translator.Handlers
{
    /// <summary>
    /// https://github.com/mtokar3v/ReversoAPI-NET
    /// </summary>
    internal class ReversoTranslationHandler : ITranslationHandler
    {
        private readonly ReversoClient _reversoClient;
        private readonly ISpellingHandler _spelling;

        public ReversoTranslationHandler(ReversoClient client, ISpellingHandler spelling)
        {
            _reversoClient = client;
            _spelling = spelling;
        }

        public async Task<string> TranslateAsync(string text, bool checkSpelling = false)
        {
            string? result;

            if (checkSpelling) text = await _spelling.SpellingCheckAsync(text);

            try
            {
                TranslationData translation = await _reversoClient.Translation.GetAsync(text, Language.English, Language.Ukrainian);

                if (translation is null || translation.Translations is null) return text;

                result = string.Join(", ", translation.Translations.Select(i => i.Value));
            }
            catch { return string.Empty; }

            return result;
        }
    }
}
