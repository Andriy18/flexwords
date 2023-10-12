using FlexWords.Translator.Interfaces;
using GTranslatorAPI;

namespace FlexWords.Translator.Handlers
{
    internal class GoogleTranslationHandler : ITranslationHandler
    {
        private readonly GTranslatorAPIClient _client = new();

        public async Task<string> TranslateAsync(string text, bool checkSpelling = false)
        {
            string? result;

            try
            {
                Translation translation = await _client.TranslateAsync(Languages.en, Languages.uk, text);

                result = translation.TranslatedText;
            }
            catch { return string.Empty; }

            return result;
        }
    }
}
