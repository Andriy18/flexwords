using FlexWords.Constants;
using FlexWords.Entities.Enums;

namespace FlexWords.Entities.Structs
{
    public struct Word
    {
        public readonly static Word Default = new();
        public readonly static Word Space = new(Words.WhiteSpace, WordType.WhiteSpace);

        public WordType Type;
        public string? Value;

        public Word(string? value, WordType type)
        {
            Value = value;
            Type = type;
        }

        public override readonly string ToString()
        {
            return string.Format("{0} '{1}'", Type, Value);
        }
    }
}
