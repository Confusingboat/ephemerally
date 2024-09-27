global using static Ephemerally.Azure.Cosmos.Xunit.Globals;
using System.Diagnostics.CodeAnalysis;
using System.Net.Sockets;

namespace Ephemerally.Azure.Cosmos.Xunit;

[SuppressMessage("ReSharper", "UseConfigureAwaitFalse")]
internal static class Globals
{
    public static Task SafeCosmosDisposeAsync<T>(Func<Task<T>> getT) where T : class =>
        IgnoreSocketException(async () =>
        {
            var t = await getT();
            // ReSharper disable once MethodHasAsyncOverload
            if (!await t.TryDisposeAsync()) t.TryDispose();
        });

    public static async Task IgnoreSocketException(Func<Task> action)
    {
        try
        {
            await action();
        }
        catch (HttpRequestException ex) when (ex.InnerException is SocketException { SocketErrorCode: SocketError.ConnectionRefused })
        {
            // This can occur if the emulator or instance is not accessible; do nothing.
        }
    }
}