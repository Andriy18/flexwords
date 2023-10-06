namespace FlexWords.Helpers
{
    public static class FileHelper
    {
        public static string[] ReadAndRemoveEmptyLines(string? filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException(filePath);
            }

            return File.ReadAllLines(filePath)
                .Select(line => line.Trim())
                .Where(line => !string.IsNullOrEmpty(line))
                .ToArray();
        }
    }
}