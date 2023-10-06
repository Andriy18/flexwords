using FlexWords.Translator.Handlers;
using FlexWords.Translator.Interfaces;
using ReversoAPI;

namespace FlexWords.Translator
{
    public sealed class TranslatorHandler : ITranslatorHandler
    {
        private readonly ReversoClient _reversoClient = new();
        private readonly ITranslationHandler _googleTranslation;
        private readonly ITranslationHandler _reversoTranslation;
        private readonly IPronunciationHandler _pronunciation;
        private readonly ISynonymsHandler _synonyms;
        private readonly ISpellingHandler _spelling;

        public TranslatorHandler()
        {
            _spelling = new SpellingHandler(_reversoClient);
            _googleTranslation = new GoogleTranslationHandler();
            _reversoTranslation = new ReversoTranslationHandler(_reversoClient, _spelling);
            _pronunciation = new PronunciationHandler(_reversoClient, _spelling);
            _synonyms = new SynonymsHandler(_reversoClient, _spelling);
        }

        public bool UseGoogleTranslate { get; set; } = true;
        public bool CheckSpelling { get; set; } = false;

        public async Task<IEnumerable<Synonym>> GetSynonymsAsync(string text)
        {
            return await _synonyms.GetSynonymsAsync(text, CheckSpelling);
        }

        public async Task<MP3SoundPlayer> PronunciationAsync(string text)
        {
            return await _pronunciation.PronunciationAsync(text, CheckSpelling);
        }

        public async Task<string> TranslateAsync(string text)
        {
            if (UseGoogleTranslate) return await _googleTranslation.TranslateAsync(text);
            else return await _reversoTranslation.TranslateAsync(text, CheckSpelling);
        }

        public void Dispose()
        {
            _pronunciation.Dispose();
            _reversoClient.Dispose();
        }
    }
}
