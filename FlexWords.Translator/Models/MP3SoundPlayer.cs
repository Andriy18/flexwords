using NAudio.Wave;

namespace FlexWords.Translator
{
    public class MP3SoundPlayer : IDisposable
    {
        private readonly WaveOutEvent? _waveOut;
        private readonly Mp3FileReader? _reader;

        private MP3SoundPlayer() { }
        public MP3SoundPlayer(Stream stream)
        {
            _reader = new(stream);
            _waveOut = new();
            _waveOut.Init(_reader);
            _waveOut.PlaybackStopped += OnPlaybackStopped;
        }

        public bool IsPlaying => _waveOut?.PlaybackState == PlaybackState.Playing;

        public void Stop()
        {
            _waveOut?.Stop();
        }
        public void Play()
        {
            _waveOut?.Play();
        }
        public void Pause()
        {
            _waveOut?.Pause();
        }
        public void Dispose()
        {
            if (_waveOut is not null)
            {
                _waveOut.PlaybackStopped -= OnPlaybackStopped;
            }

            _reader?.Close();
            _reader?.Dispose();
            _waveOut?.Stop();
            _waveOut?.Dispose();
        }

        private void OnPlaybackStopped(object? sender, StoppedEventArgs e)
        {
            _reader?.Seek(0, SeekOrigin.Begin);
        }

        public readonly static MP3SoundPlayer Empty = new();
    }
}
