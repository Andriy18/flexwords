using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using FlexWords.Constants;

namespace FlexWords.Helpers
{
    public static class DataHelper
    {
        public static string ComputeHash(string filePath)
        {
            string hash;
            string[] lines = FileHelper.ReadAndRemoveEmptyLines(filePath);
            string generalData = string.Join(string.Empty, lines);
            byte[] data = Encoding.UTF8.GetBytes(generalData);

            using (var md5 = MD5.Create())
            {
                byte[] byteArray = md5.ComputeHash(data);
                hash = Convert.ToBase64String(byteArray);
            }

            return hash;
        }

        public static Dictionary<char, int> GetDictOfSymbols(string[] text)
        {
            var symbols = new Dictionary<char, int>();

            foreach (string line in text)
            {
                char[] charArray = line.ToCharArray();

                foreach (char symbol in charArray)
                {
                    if (symbols.ContainsKey(symbol))
                    {
                        symbols[symbol]++;
                    }
                    else
                    {
                        symbols.Add(symbol, 1);
                    }
                }
            }

            return symbols
                .OrderByDescending(i => i.Value)
                .ToDictionary(i => i.Key, i => i.Value);
        }

        public static bool CheckForCharacterCompatibility(string[] lines)
        {
            foreach (string line in lines)
            {
                foreach (char item in line)
                {
                    if (!Words.DictionaryCheck.Contains(item))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public static char GetNormalChar(char symbol)
        {
            string character = RemoveDiacritics(symbol.ToString());

            foreach (char item in Words.ABCLower + Words.ABCUpper)
            {
                if (item.Equals(character[0]))
                {
                    return item;
                }
            }

            return character[0];
        }

        public static string RemoveDiacritics(string text)
        {
            string normalizedString = text.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder(capacity: normalizedString.Length);

            foreach (char c in normalizedString)
            {
                UnicodeCategory unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);

                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder
                .ToString()
                .Normalize(NormalizationForm.FormC);
        }
    }
}
