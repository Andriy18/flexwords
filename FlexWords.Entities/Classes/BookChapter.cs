using FlexWords.Constants;

namespace FlexWords.Entities.Classes
{
    public sealed class BookChapter
    {
        private readonly Book _parent;
        private string _title = Words.DefaultChapter;

        public BookChapter(Book book)
        {
            _parent = book;
        }

        public BookChapter(Book book, string title) : this(book)
        {
            _parent = book;
            Title = title;
        }

        public string Title
        {
            get => _title;
            set => _title = value;
        }
        public Book Parent => _parent;
        public int ParagraphCount { get; set; }

        public override string ToString()
        {
            return string.Format("'{0}'", Title);
        }
    }
}
