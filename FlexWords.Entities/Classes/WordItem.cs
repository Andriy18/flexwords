using FlexWords.Entities.Structs;

namespace FlexWords.Entities.Classes
{
    public sealed class WordItem
    {
        public bool Flag { get; set; }
        public Word Value { get; }

        public char[] Letters => Value.Value?.ToCharArray() ?? Array.Empty<char>();

        public WordItem(Word word)
        {
            Value = word;
        }

        public WordItem(Word word, bool flag) : this(word)
        {
            Flag = flag;
        }

        public override string ToString()
        {
            return Value.ToString();
        }
    }
}
