using FlexWords.Entities.Classes;
using FlexWords.Entities.Enums;
using FlexWords.Entities.Structs;

namespace FlexWords.Extensions
{
    public static class WordsExtensions
    {
        public static Sentence[] SplitToSentences(this Word[] source)
        {
            if (source.Length is 0) throw new ArgumentException("Must be more than zero words");

            if (!source.Any(i => i.Type is WordType.SentenceSeparator))
            {
                return new Sentence[1] { new Sentence(source) };
            }

            bool attachLeft = false;
            var result = new Queue<Sentence>();
            var words = new Queue<Word>();

            for (int index = 0; index < source.Length; index++)
            {
                Word word = source[index];

                if (word.Type is WordType.Dialog) attachLeft = !attachLeft;
                words.Enqueue(word);

                if (word.Type is WordType.SentenceSeparator)
                {
                    if (attachLeft)
                    {
                        for (int j = index + 1; j < source.Length; j++)
                        {
                            word = source[j];

                            if (word.Type is WordType.Dialog)
                            {
                                attachLeft = !attachLeft;
                                words.Enqueue(word);
                                index++;

                                break;
                            }
                            else if (word.Type is WordType.WhiteSpace)
                            {
                                words.Enqueue(word);
                                index++;

                                continue;
                            }
                            else break;
                        }
                    }

                    result.Enqueue(new Sentence(words.Trim()));

                    words = new Queue<Word>();
                }
            }

            if (words.Count > 0)
            {
                result.Enqueue(new Sentence(words.Trim()));
            }

            return result.ToArray();
        }

        public static IReadOnlyList<WordItem[]> GroupIntoInseparableParts(this IReadOnlyList<WordItem> items)
        {
            var result = new List<WordItem[]>();
            var entities = new Stack<WordItem>(items.Reverse());
            var list = new List<WordItem>();

            while (entities.Count > 0)
            {
                WordItem element = entities.Pop();

                if (element.Value.Type == WordType.WhiteSpace)
                {
                    if (list.Count > 0)
                    {
                        result.Add(list.ToArray());
                        list = new List<WordItem>();
                    }
                }
                else
                {
                    if (list.Any(i => i.Value.Type == WordType.Text) &&
                        element.Value.Type == WordType.Text)
                    {
                        result.Add(list.ToArray());
                        list = new List<WordItem>();
                    }

                    list.Add(element);
                }
            }

            if (list.Count > 0)
            {
                result.Add(list.ToArray());
            }

            return result;
        }

        public static IEnumerable<Word> Trim(this IEnumerable<Word> words)
        {
            return words
                .SkipWhile(i => i.Type == WordType.WhiteSpace).Reverse()
                .SkipWhile(i => i.Type == WordType.WhiteSpace).Reverse();
        }

        public static IEnumerable<WordItem> ToWordItems(this IReadOnlyList<Word> words, bool flag)
        {
            return words
                .Where(i => i.Type != WordType.WhiteSpace)
                .Select(i => new WordItem(i, flag));
        }
    }
}
