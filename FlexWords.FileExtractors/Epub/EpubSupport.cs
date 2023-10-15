using System.Net;
using System.Text.RegularExpressions;
using EpubSharp;
using FlexWords.Constants;
using FlexWords.Entities;

namespace FlexWords.FileExtractors.Epub
{
    public static class EpubSupport
    {
        public static RawFileData Extract(this EpubBook book)
        {
            var result = new RawFileData();
            var data = new Dictionary<string, List<string>>();

            bool useDefaultChapter = false;
            string activeChapter = string.Empty;
            var chapters = book.TableOfContents
                .ToDictionary(i => i.FileName, i => i.Title);

            if (chapters.Count is 0)
            {
                useDefaultChapter = true;
                activeChapter = Words.DefaultChapter;
                data.Add(activeChapter, new List<string>());
            }

            foreach (EpubTextFile item in book.SpecialResources.HtmlInReadingOrder)
            {
                if (!useDefaultChapter && chapters.ContainsKey(item.FileName))
                {
                    activeChapter = chapters[item.FileName];
                    data.Add(activeChapter, new List<string>());

                    data[activeChapter].AddRange(item.ExtractLines(activeChapter));

                    continue;
                }

                if (data.Count > 0)
                {
                    data[activeChapter].AddRange(item.ExtractLines());
                }
            }

            result.Data = data;

            return result;
        }

        private static IReadOnlyList<string> ExtractLines(this EpubTextFile file, string? skipLine = null)
        {
            var lines = new List<string>();

            try
            {
                string paragraph = EpubDecompiler.ExtractText(file.TextContent).Trim();

                foreach (var line in paragraph.Split('\n', StringSplitOptions.RemoveEmptyEntries).Select(i => i.Trim()))
                {
                    if (skipLine != null && skipLine == line) continue;

                    if (string.IsNullOrEmpty(line)) continue;

                    lines.Add(line);
                }
            }
            catch { }

            return lines;
        }

        private class EpubDecompiler
        {
            private static readonly RegexOptions RegexOptions = RegexOptions.ExplicitCapture | RegexOptions.IgnorePatternWhitespace | RegexOptions.CultureInvariant;

            private static readonly RegexOptions RegexOptionsIgnoreCase = RegexOptions.IgnoreCase | RegexOptions;

            private static readonly RegexOptions RegexOptionsIgnoreCaseSingleLine = RegexOptions.Singleline | RegexOptionsIgnoreCase;

            private static readonly RegexOptions RegexOptionsIgnoreCaseMultiLine = RegexOptions.Multiline | RegexOptionsIgnoreCase;

            public static string ExtractText(string html)
            {
                if (string.IsNullOrWhiteSpace(html))
                {
                    throw new ArgumentNullException("html");
                }

                html = html.Trim();
                html = Regex.Replace(html, "\\r\\n?|\\n", "");
                Match match = Regex.Match(html, "<body[^>]*>.+</body>", RegexOptionsIgnoreCaseSingleLine);
                if (!match.Success)
                {
                    return "";
                }

                return ClearText(match.Value).Trim(' ', '\r', '\n');
            }
            private static string ClearText(string text)
            {
                if (text == null)
                {
                    return null;
                }

                return DecodeHtmlSymbols(RemoveHtmlTags(ReplaceBlockTagsWithNewLines(text)));
            }
            private static string RemoveHtmlTags(string text)
            {
                if (text != null)
                {
                    return Regex.Replace(text, "</?(\\w+|\\s*!--)[^>]*>", " ", RegexOptions);
                }

                return null;
            }
            private static string ReplaceBlockTagsWithNewLines(string text)
            {
                if (text != null)
                {
                    return Regex.Replace(text, "(?<!^\\s*)<(p|div|h1|h2|h3|h4|h5|h6)[^>]*>", "\n", RegexOptionsIgnoreCaseMultiLine);
                }

                return null;
            }
            private static string DecodeHtmlSymbols(string text)
            {
                if (text == null)
                {
                    return null;
                }

                text = Regex.Replace(new Regex("(?<defined>(&nbsp|&quot|&mdash|&ldquo|&rdquo|\\&\\#8211|\\&\\#8212|&\\#8230|\\&\\#171|&laquo|&raquo|&amp);?)|(?<other>\\&\\#\\d+;?)", RegexOptionsIgnoreCase).Replace(text, new MatchEvaluator(SpecialSymbolsEvaluator)), "\\ {2,}", " ", RegexOptions);
                text = WebUtility.HtmlDecode(text);
                return text;
            }
            private static string SpecialSymbolsEvaluator(Match m)
            {
                if (!m.Groups["defined"].Success)
                {
                    return " ";
                }

                return m.Groups["defined"].Value.ToLower() switch
                {
                    "&nbsp;" => " ",
                    "&nbsp" => " ",
                    "&quot;" => "\"",
                    "&quot" => "\"",
                    "&mdash;" => " ",
                    "&mdash" => " ",
                    "&ldquo;" => "\"",
                    "&ldquo" => "\"",
                    "&rdquo;" => "\"",
                    "&rdquo" => "\"",
                    "&#8211;" => "-",
                    "&#8211" => "-",
                    "&#8212;" => "-",
                    "&#8212" => "-",
                    "&#8230" => "...",
                    "&#171;" => "\"",
                    "&#171" => "\"",
                    "&laquo;" => "\"",
                    "&laquo" => "\"",
                    "&raquo;" => "\"",
                    "&raquo" => "\"",
                    "&amp;" => "&",
                    "&amp" => "&",
                    _ => " ",
                };
            }
        }
    }
}
