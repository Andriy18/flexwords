using FlexWords.Entities.Structs;

namespace FlexWords.Entities.Classes
{
    public sealed class Book
    {
        private Bookmark _bkmark;

        public Book(string path)
        {
            FilePath = path;
            Title = Path.GetFileNameWithoutExtension(path);
        }

        public string Title { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public Bookmark Bkmark
        {
            get => _bkmark;
            set
            {
                if (Paragraphs.Count is 0 || Chapters.Count is 0) return;

                int par = ValidateParagraph(value.paragraph);
                int sen = ValidateSentence(par, value.sentence);

                _bkmark = new Bookmark(par, sen);
            }
        }

        public bool Next()
        {
            if (Paragraphs[_bkmark.paragraph].Sentences.Length > _bkmark.sentence + 1)
            {
                _bkmark = new Bookmark(_bkmark.paragraph, _bkmark.sentence + 1);

                return true;
            }

            if (_bkmark.paragraph + 1 < Paragraphs.Count)
            {
                _bkmark = new Bookmark(_bkmark.paragraph + 1, 0);

                return false;
            }

            return true;
        }
        public bool Back()
        {
            if (_bkmark.sentence > 0)
            {
                _bkmark = new Bookmark(_bkmark.paragraph, _bkmark.sentence - 1);

                return true;
            }

            if (_bkmark.paragraph > 0)
            {
                int sen = Paragraphs[_bkmark.paragraph - 1].Sentences.Length - 1;
                _bkmark = new Bookmark(_bkmark.paragraph - 1, sen);

                return false;
            }

            return true;
        }

        public int ValidateParagraph(int paragraph)
        {
            return Math.Clamp(paragraph, 0, Paragraphs.Count - 1);
        }
        public int ValidateSentence(int paragraph, int sentence)
        {
            return Math.Clamp(sentence, 0, Paragraphs[paragraph].Sentences.Length - 1);
        }

        public List<BookParagraph> Paragraphs { get; } = new();
        public List<BookChapter> Chapters { get; } = new();

        public int SentenseCount { get; set; }
        public int WordCount { get; set; }
        public float PageCount { get; set; }

        public override string ToString()
        {
            return string.Format("'{0}' {1:f3}", Title, PageCount);
        }
    }
}
