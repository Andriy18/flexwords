namespace FlexWords.Extensions.Specific
{
    public static class ObjectExtensions
    {
        public static void CheckDispose(this object obj)
        {
            if (obj is IDisposable disposable) disposable.Dispose();
        }
    }
}
