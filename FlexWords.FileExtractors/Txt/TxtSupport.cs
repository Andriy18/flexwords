using FlexWords.Constants;
using FlexWords.Entities;
using FlexWords.Helpers;

namespace FlexWords.FileExtractors.Txt
{
    public static class TxtSupport
    {
        public static RawFileData Extract(string txtFilePath, string? chapterSeparator = null)
        {
            var result = new RawFileData();
            var data = new Dictionary<string, List<string>>();

            bool useDefaultChapter = false;
            string activeChapter = string.Empty;
            string[] lines = FileHelper.ReadAndRemoveEmptyLines(txtFilePath);

            if (string.IsNullOrEmpty(chapterSeparator) ||
                !lines.Any(line => line.StartsWith(chapterSeparator)))
            {
                chapterSeparator = string.Empty;
                useDefaultChapter = true;
                activeChapter = Words.DefaultChapter;
                data.Add(activeChapter, new List<string>());
            }

            foreach (string line in lines)
            {
                if (!useDefaultChapter && line.StartsWith(chapterSeparator))
                {
                    string chapterName = string.Concat(line.Skip(3));

                    data.Add(chapterName, new List<string>());
                    activeChapter = chapterName;

                    continue;
                }

                if (data.Count > 0)
                {
                    data[activeChapter].Add(line);
                }
            }

            result.Data = data;

            return result;
        }
    }
}
