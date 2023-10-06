using FlexWords.Entities.Enums;

namespace FlexWords.Entities.Classes
{
    public sealed class BookParagraph
    {
        private readonly Sentence[] _sentences;
        private readonly BookChapter _parent;

        public BookParagraph(BookChapter chapter, Sentence[] sentences)
        {
            chapter.ParagraphCount++;

            _parent = chapter;
            _sentences = sentences;
        }

        public Sentence[] Sentences => _sentences;
        public BookChapter Chapter => _parent;

        public int WordCount
        {
            get => _sentences.Sum(s => s.Words.Count(w => w.Type == WordType.Text && w.Value?.Length > 1));
        }
        public float Pages { get; set; }
        public int TotalPages { get; set; }
        public float CurrentPageInterest { get; set; }
    }
}
