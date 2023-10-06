using FlexWords.Translator.Interfaces;
using ReversoAPI;

namespace FlexWords.Translator.Handlers
{
    internal class SynonymsHandler : ISynonymsHandler
    {
        private readonly ReversoClient _reversoClient;
        private readonly ISpellingHandler _spelling;

        public SynonymsHandler(ReversoClient client, ISpellingHandler spelling)
        {
            _reversoClient = client;
            _spelling = spelling;
        }

        public async Task<IEnumerable<Synonym>> GetSynonymsAsync(string text, bool checkSpelling = false)
        {
            IEnumerable<Synonym>? synonyms;

            if (checkSpelling) text = await _spelling.SpellingCheckAsync(text);

            try
            {
                SynonymsData data = await _reversoClient.Synonyms.GetAsync(text, Language.English);

                if (data is null || data.Synonyms is null) return Enumerable.Empty<Synonym>();

                synonyms = data.Synonyms.Select(i => new Synonym
                {
                    Text = i.Value,
                    Type = i.PartOfSpeech.ToString()
                });
            }
            catch { return Enumerable.Empty<Synonym>(); }

            return synonyms;
        }
    }
}
