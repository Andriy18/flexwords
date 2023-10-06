namespace FlexWords.Translator
{
    public interface ITranslatorHandler : IDisposable
    {
        bool UseGoogleTranslate { get; set; }
        bool CheckSpelling { get; set; }
        Task<MP3SoundPlayer> PronunciationAsync(string text);
        Task<IEnumerable<Synonym>> GetSynonymsAsync(string text);
        Task<string> TranslateAsync(string text);
    }
}
