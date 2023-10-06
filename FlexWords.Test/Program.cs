using FlexWords.Extensions;
using FlexWords.Helpers;
using FlexWords.Entities.Classes;
using FlexWords.Entities.Structs;

namespace FlexWords.Test
{
    internal class Program
    {
        static void Main()
        {
            
        }

        public static void Show(string text)
        {
            Console.BackgroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.WriteLine(text);
            Console.ResetColor();
        }

        public static void Show(IReadOnlyList<Sentence> data)
        {
            bool color = false;

            foreach (Sentence sentense in data)
            {
                Console.BackgroundColor = color ? ConsoleColor.Magenta : ConsoleColor.DarkBlue;

                Console.Write(string.Concat(sentense.Words.Select(w => w.Value)));

                Console.ResetColor();
                Console.Write(" ");
                Console.WriteLine();

                color = !color;
            }

            Console.ResetColor();
            Console.Write(" ");
            Console.WriteLine();
        }

        public static void Show(Word[] data)
        {
            bool color = false;

            foreach (Word word in data)
            {
                if (color)
                {
                    Console.BackgroundColor = ConsoleColor.Magenta;
                }
                else
                {
                    Console.BackgroundColor = ConsoleColor.DarkBlue;
                }

                Console.Write(word.Value);

                color = !color;
            }

            Console.ResetColor();
            Console.Write(" ");
            Console.WriteLine();
        }

        public static void RandomSentense(string path)
        {
            string[] data = FileHelper.ReadAndRemoveEmptyLines(path);
            Random random = new Random(DateTime.Now.Millisecond);

            while (true)
            {
                Console.Clear();
                string line = data[random.Next(data.Length)];
                Show(BookExtensions.SplitToWords(line).SplitToSentences());
                Show(line);

                if (Console.ReadKey().Key == ConsoleKey.Spacebar)
                {
                    break;
                }
            }
        }

        public static bool Check(Sentence sentence, string result)
        {
            return string.Concat(sentence.Words.Select(i => i.Value)) == result;
        }

        public static void CheckFactory()
        {
            string text = "Lawrence couldn’t help laughing at her naive question, " +
                "which earned him a sharp, angry look. “If a merchant like me showed " +
                "up at the mint, the only greeting I’d get would be the business end " +
                "of a spear. No, we’re going to see the cambist.”";
            string text2 = "He swallowed his shock and managed to get some words out. " +
                "“That is probably my companion, Holo. She acted as a decoy so I could " +
                "make it here.”";

            Sentence[] sentence = BookExtensions.SplitToWords(text).SplitToSentences();

            Console.WriteLine(Check(sentence[0], "Lawrence couldn’t help laughing at her naive question, which earned him a sharp, angry look."));
            Console.WriteLine(Check(sentence[1], "“If a merchant like me showed up at the mint, the only greeting I’d get would be the business end of a spear."));
            Console.WriteLine(Check(sentence[2], "No, we’re going to see the cambist.”"));

            sentence = BookExtensions.SplitToWords(text2).SplitToSentences();

            Console.WriteLine(Check(sentence[0], "He swallowed his shock and managed to get some words out."));
            Console.WriteLine(Check(sentence[1], "“That is probably my companion, Holo."));
            Console.WriteLine(Check(sentence[2], "She acted as a decoy so I could make it here.”"));
        }
    }
}