namespace FlexWords.Entities
{
    public sealed class RawFileData
    {
        public bool HasData => Data is not null && Data.Count > 0;
        public IReadOnlyDictionary<string, List<string>>? Data { get; set; }

        public static readonly RawFileData Empty = new();
    }
}
