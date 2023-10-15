using EpubSharp;
using FlexWords.Constants;
using FlexWords.Entities;
using FlexWords.Entities.Enums;
using FlexWords.FileExtractors.Epub;
using FlexWords.FileExtractors.Txt;

namespace FlexWords.FileExtractors
{
    public static class FileExtractor
    {
        public static RawFileData Extract(string filePath, FileFormat format)
        {
            if (format is FileFormat.Txt)
            {
                return TxtSupport.Extract(filePath, Words.ChapterSeparator);
            }
            
            if (format is FileFormat.Epub)
            {
                return EpubReader.Read(filePath).Extract();
            }

            return RawFileData.Empty;
        }
    }
}
