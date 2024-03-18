using Microsoft.Azure.Cosmos;
using System.Diagnostics.CodeAnalysis;
using System.Net.Sockets;
using Xunit;

namespace Ephemerally.Azure.Cosmos.Xunit;

[SuppressMessage("ReSharper", "UseConfigureAwaitFalse")]
public class CosmosClientFixture :
    IAsyncDisposable,
    IAsyncLifetime
{
    private readonly Lazy<Task<CosmosClient>> _client;

    public CosmosClient Client => _client.Value.Result;

    protected Task<CosmosClient> GetClient() => _client.Value;

    public CosmosClientFixture()
    {
        _client = new(CreateClientAsync);
    }

    protected virtual Task<CosmosClient> CreateClientAsync() => Task.FromResult(CosmosEmulator.GetClient());

    public virtual Task InitializeAsync() => _client.Value;

    public virtual async Task DisposeAsync()
    {
        if (!_client.IsValueCreated) return;

        await IgnoreSocketException(async () =>
        {
            var client = await GetClient();
            client.Dispose();
        });
    }

    async ValueTask IAsyncDisposable.DisposeAsync() =>
        await ((IAsyncLifetime)this).DisposeAsync();

    protected static async Task IgnoreSocketException(Func<Task> action)
    {
        try
        {
            await action();
        }
        catch (HttpRequestException ex) when (ex.InnerException is SocketException { SocketErrorCode: SocketError.ConnectionRefused })
        {
            // If the emulator or instance is not accessible, we don't need to do anything
        }
    }
}