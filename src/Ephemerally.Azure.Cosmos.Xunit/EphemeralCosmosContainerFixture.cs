using Microsoft.Azure.Cosmos;
using Xunit;

namespace Ephemerally.Azure.Cosmos.Xunit;

internal abstract class EphemeralCosmosContainerFixture :
    IAsyncDisposable,
    IAsyncLifetime
{
    private Lazy<ValueTask<EphemeralCosmosContainer>> _container;

    protected EphemeralCosmosContainerFixture(
        Database database,
        EphemeralCreationOptions options)
    {
        _container = new(async () => await database.CreateEphemeralContainerAsync(options).ConfigureAwait(false));
    }

    async Task IAsyncLifetime.InitializeAsync() =>
        await _container.Value.ConfigureAwait(false);

    async Task IAsyncLifetime.DisposeAsync() =>
        await ((IAsyncDisposable)this).DisposeAsync().ConfigureAwait(false);

    ValueTask IAsyncDisposable.DisposeAsync() =>
        _container.Value.Result.DisposeAsync();
}
