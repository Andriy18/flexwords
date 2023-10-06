using FlexWords.Translator.Interfaces;
using ReversoAPI;
using NAudio.Wave;

namespace FlexWords.Translator.Handlers
{
    internal class PronunciationHandler : IPronunciationHandler
    {
        private readonly ReversoClient _reversoClient;
        private readonly ISpellingHandler _spelling;
        private readonly WaveOutEvent _waveOut = new();

        public PronunciationHandler(ReversoClient client, ISpellingHandler spelling)
        {
            _reversoClient = client;
            _spelling = spelling;
        }

        public void Dispose()
        {
            _waveOut.Stop();
            _waveOut.Dispose();
        }

        public async Task<MP3SoundPlayer> PronunciationAsync(string text, bool checkSpelling = false)
        {
            MP3SoundPlayer? player;

            if (checkSpelling) text = await _spelling.SpellingCheckAsync(text);

            try
            {
                Stream stream = await _reversoClient.Pronunciation.GetAsync(text, Language.English);

                player = new MP3SoundPlayer(stream);
            }
            catch { return MP3SoundPlayer.Empty; }

            return player;
        }
    }
}
