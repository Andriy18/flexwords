using FlexWords.Constants;
using FlexWords.Entities.Classes;
using FlexWords.Entities.Enums;
using FlexWords.Entities.Structs;
using FlexWords.Helpers;

namespace FlexWords.Extensions
{
    public static class BookExtensions
    {
        public static void FillContent(this Book book)
        {
            string[] lines = FileHelper.ReadAndRemoveEmptyLines(book.FilePath);
            BookChapter? chapter = null;

            foreach (string line in lines)
            {
                if (line.StartsWith(Words.ChapterSeparator))
                {
                    chapter = new BookChapter(book, string.Concat(line.Skip(3)));
                    book.Chapters.Add(chapter);

                    continue;
                }

                if (chapter is null)
                {
                    chapter = new BookChapter(book);
                    book.Chapters.Add(chapter);
                }

                Sentence[] sentences = SplitToWords(line).SplitToSentences();
                BookParagraph paragraph = new(chapter, sentences);
                book.Paragraphs.Add(paragraph);
            }
        }

        public static void FillStatistics(this Book book)
        {
            book.SentenseCount = book.Paragraphs.Sum(i => i.Sentences.Length);
            book.WordCount = book.Paragraphs.Sum(i => i.WordCount);

            float progress = 0;

            foreach (BookParagraph paragraph in book.Paragraphs)
            {
                paragraph.TotalPages = (int)progress;
                paragraph.CurrentPageInterest = progress - paragraph.TotalPages;
                paragraph.Pages = paragraph.WordCount / 199.814f;

                progress += paragraph.Pages;
            }

            book.PageCount = progress;
        }

        public static bool MoveToPage(this Book book, float page)
        {
            bool result;
            bool found = false;
            int paragraphIndex = 0, par;
            page = Math.Clamp(page, 0, book.PageCount);

            foreach (BookParagraph paragraph in book.Paragraphs)
            {
                if (page <= paragraph.TotalPages + paragraph.CurrentPageInterest)
                {
                    found = true;
                    paragraphIndex = book.Paragraphs.IndexOf(paragraph);

                    break;
                }
            }

            if (!found)
            {
                par = book.Paragraphs.Count - 1;
                result = par == book.Bkmark.paragraph;
                book.Bkmark = new Bookmark(par, 0);

                return result;
            }

            par = paragraphIndex;
            result = par == book.Bkmark.paragraph;
            book.Bkmark = new Bookmark(par, 0);

            return result;
        }

        public static float CalculateTotalPages(this Book book, Bookmark from, Bookmark to)
        {
            if (book.Paragraphs.Count is 0 || book.Chapters.Count is 0) return 0;

            int par = book.ValidateParagraph(to.paragraph);
            int sen = book.ValidateSentence(par, to.sentence);
            to = new Bookmark(par, sen);

            par = book.ValidateParagraph(from.paragraph);
            sen = book.ValidateSentence(par, from.sentence);
            from = new Bookmark(par, sen);

            if (from > to) return 0;

            if (from == to) return book.Paragraphs[to.paragraph].Pages;

            BookParagraph paragraph = book.Paragraphs[to.paragraph];
            float result = paragraph.TotalPages + paragraph.CurrentPageInterest + paragraph.Pages;

            paragraph = book.Paragraphs[from.paragraph];
            result -= paragraph.TotalPages + paragraph.CurrentPageInterest;

            return result;
        }

        public static float GetTotalPagesCount(this BookParagraph paragraph)
        {
            return paragraph.TotalPages + paragraph.CurrentPageInterest;
        }

        public static IReadOnlyList<WordItem> CurrentWords(this Book book)
        {
            BookParagraph current = book.Paragraphs[book.Bkmark.paragraph];
            var items = new Queue<WordItem>();

            for (int index = 0; index < current.Sentences.Length; index++)
            {
                bool flag = index == book.Bkmark.sentence;

                foreach (WordItem item in current.Sentences[index].Words.ToWordItems(flag))
                {
                    items.Enqueue(item);
                }
            }

            return items.ToArray();
        }

        public static IReadOnlyList<WordItem[]> CurrentWordGroups(this Book book)
        {
            var result = new Queue<WordItem[]>();
            BookParagraph current = book.Paragraphs[book.Bkmark.paragraph];

            for (int index = 0; index < current.Sentences.Length; index++)
            {
                bool flag = index == book.Bkmark.sentence;

                WordItem[] items = current.Sentences[index].Words.ToWordItems(flag).ToArray();

                foreach (WordItem[] groups in items.GroupIntoInseparableParts())
                {
                    result.Enqueue(groups);
                }
            }

            return result.ToArray();
        }

        public static Word[] SplitToWords(string source)
        {
            var result = new List<Word>();
            string[] parts = source.Split(
                new string[1] { Words.WhiteSpace },
                StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < parts.Length; i++)
            {
                result.AddRange(SplitToWordsRecursively(parts[i]));

                if (i == parts.Length - 1)
                {
                    continue;
                }

                result.Add(Word.Space);
            }

            return result.ToArray();
        }

        private static Word[] SplitToWordsRecursively(string source)
        {
            var result = new List<Word>();

            if (source == string.Empty)
            {
                return result.ToArray();
            }

            char[] chars = source.ToCharArray();

            for (int i = 0; i < chars.Length; i++)
            {
                string compare = chars[i].ToString();

                if (Words.LettersCheck.Contains(chars[i]) ||
                    Words.Dashes.Contains(chars[i]) && i != 0 && i != chars.Length - 1 ||
                    Words.Apostrophes.Contains(chars[i]))
                {
                    continue;
                }
                else
                {
                    char normalChar = DataHelper.GetNormalChar(chars[i]);

                    if (Words.LettersCheck.Contains(normalChar))
                    {
                        continue;
                    }

                    if (i is not 0)
                    {
                        string pastPart = string.Concat(chars.Take(i));
                        result.Add(new Word(pastPart, WordType.Text));
                    }

                    if (Words.Dot.Contains(chars[i]))
                    {
                        if (i > 1)
                        {
                            string expression = chars[i - 2].ToString() + chars[i - 1].ToString();

                            if (expression == "Mr")
                            {
                                result.Add(new Word(Words.Dot, WordType.Sign));

                                continue;
                            }
                        }

                        string dots = Words.Dot;

                        for (int j = i + 1; j < chars.Length; j++, i++)
                        {
                            if (Words.Dot.Contains(chars[j]))
                            {
                                dots += Words.Dot;
                            }
                            else
                            {
                                break;
                            }
                        }

                        result.Add(new Word(dots, dots == Words.Dot
                            ? WordType.SentenceSeparator
                            : WordType.Sign));
                    }
                    else if (Words.SentenceEnd.Contains(chars[i]))
                    {
                        result.Add(new Word(compare, WordType.SentenceSeparator));
                    }
                    else if (Words.Dialogue.Contains(chars[i]))
                    {
                        result.Add(new Word(compare, WordType.Dialog));
                    }
                    else if (Words.BaseSymbols.Contains(chars[i]))
                    {
                        result.Add(new Word(compare, WordType.Sign));
                    }
                    else
                    {
                        result.Add(new Word(compare, WordType.None));
                    }

                    string nextPart = string.Concat(chars.Skip(compare.Length + i));
                    result.AddRange(SplitToWordsRecursively(nextPart));

                    break;
                }
            }

            if (result.Count is 0)
            {
                result.Add(new Word(source, WordType.Text));
            }

            return result.ToArray();
        }
    }
}