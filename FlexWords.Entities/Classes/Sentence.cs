using FlexWords.Entities.Structs;

namespace FlexWords.Entities.Classes
{
    public sealed class Sentence
    {
        private readonly Word[] _words;

        public Sentence(IEnumerable<Word> words)
        {
            _words = words.ToArray();
        }

        public Word[] Words => _words;
    }
}
