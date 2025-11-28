namespace Ephemerally;

internal static class InternalExtensions
{
    internal static T OrDefault<T>(this T options) where T : EphemeralOptions, new() =>
        options ?? new T();

    extension<T>(T self) where T : class
    {
        public async ValueTask<bool> TryDisposeAsync()
        {
            if (self is not IAsyncDisposable disposable)
                return false;

            await disposable.DisposeAsync().ConfigureAwait(false);
            return true;
        }

        public bool TryDispose()
        {
            if (self is not IDisposable disposable)
                return false;

            disposable.Dispose();
            return true;
        }
    }
}