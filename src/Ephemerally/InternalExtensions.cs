namespace Ephemerally;

internal static class InternalExtensions
{
    internal static T OrDefault<T>(this T options) where T : EphemeralOptions, new() =>
        options ?? new T();

    public static async ValueTask<bool> TryDisposeAsync<T>(this T self) where T : class
    {
        if (self is not IAsyncDisposable disposable)
            return false;

        await disposable.DisposeAsync().ConfigureAwait(false);
        return true;
    }

    public static bool TryDispose<T>(this T self) where T : class
    {
        if (self is not IDisposable disposable)
            return false;

        disposable.Dispose();
        return true;
    }
}