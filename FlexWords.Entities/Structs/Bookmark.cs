using System.Diagnostics.CodeAnalysis;

namespace FlexWords.Entities.Structs
{
    public struct Bookmark
    {
        public static readonly Bookmark Empty = new();

        public Bookmark(int par, int sen)
        {
            sentence = sen;
            paragraph = par;
        }

        public Bookmark(string data)
        {
            Bookmark instance = Parse(data);

            sentence = instance.sentence;
            paragraph = instance.paragraph;
        }

        public int sentence;
        public int paragraph;

        public static Bookmark Parse(string data)
        {
            int sen, par;

            try
            {
                string[] items = data.Split(':');

                par = int.Parse(items[0]);
                sen = int.Parse(items[1]);
            }
            catch
            {
                return Empty;
            }

            return new Bookmark
            {
                paragraph = par,
                sentence = sen
            };
        }

        public override readonly string ToString()
        {
            return string.Format("{0}:{1}", paragraph, sentence);
        }

        public override readonly int GetHashCode()
        {
            return HashCode.Combine(paragraph, sentence);
        }

        public override readonly bool Equals([NotNullWhen(true)] object? obj)
        {
            if (obj is not Bookmark bookmark) return false;

            return GetHashCode() == bookmark.GetHashCode();
        }

        public static bool operator ==(Bookmark left, Bookmark right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Bookmark left, Bookmark right)
        {
            return !(left == right);
        }

        public static bool operator >(Bookmark left, Bookmark right)
        {
            return left.paragraph > right.paragraph ||
                left.paragraph == right.paragraph && left.sentence > right.sentence;
        }

        public static bool operator <(Bookmark left, Bookmark right)
        {
            return right > left;
        }
    }
}
