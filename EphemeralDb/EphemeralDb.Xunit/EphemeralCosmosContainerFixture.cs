using EphemeralDb.Cosmos;
using Microsoft.Azure.Cosmos;
using Xunit;

namespace EphemeralDb.Xunit;

internal abstract class EphemeralCosmosContainerFixture :
    IAsyncDisposable,
    IAsyncLifetime
{
    private readonly EphemeralCosmosContainer _container;

    protected EphemeralCosmosContainerFixture(Database database, EphemeralCosmosContainerOptions options = default)
    {
        _container = new(database, options ?? EphemeralCosmosContainerOptions.Default);
    }

    async Task IAsyncLifetime.InitializeAsync() =>
        await _container.GetContainerAsync();

    async Task IAsyncLifetime.DisposeAsync() =>
        await ((IAsyncDisposable)this).DisposeAsync();

    ValueTask IAsyncDisposable.DisposeAsync() =>
        _container.DisposeAsync();
}
