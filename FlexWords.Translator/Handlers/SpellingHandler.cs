using FlexWords.Translator.Interfaces;
using ReversoAPI;

namespace FlexWords.Translator.Handlers
{
    internal class SpellingHandler : ISpellingHandler
    {
        private readonly ReversoClient _reversoClient;

        public SpellingHandler(ReversoClient client)
        {
            _reversoClient = client;
        }

        public async Task<string> SpellingCheckAsync(string text)
        {
            string? result;

            try
            {
                SpellingData data = await _reversoClient.Spelling.GetAsync(text, Language.English, Locale.US);
                
                if (data is null || data.Text is null) return text;
                
                result = data.Text;
            }
            catch { return text; }

            return result;
        }
    }
}
