namespace FlexWords.Translator.Interfaces
{
    internal interface IPronunciationHandler : IDisposable
    {
        Task<MP3SoundPlayer> PronunciationAsync(string text, bool checkSpelling = false);
    }
}
