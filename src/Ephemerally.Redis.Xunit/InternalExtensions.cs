namespace Ephemerally.Redis.Xunit;

internal static class InternalExtensions
{
    public static ValueTask TryDisposeAsync<T>(this T self) where T : class =>
        self is IAsyncDisposable disposable
            ? disposable.DisposeAsync()
            : new();

    public static void TryDispose<T>(this T self) where T : class
    {
        (self as IDisposable)?.Dispose();
    }
}