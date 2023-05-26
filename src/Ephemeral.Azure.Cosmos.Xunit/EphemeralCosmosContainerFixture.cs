using Ephemeral;
using Ephemeral.Azure.Cosmos;
using Microsoft.Azure.Cosmos;
using Xunit;

namespace EphemeralDb.Xunit;

internal abstract class EphemeralCosmosContainerFixture :
    IAsyncDisposable,
    IAsyncLifetime
{
    private readonly EphemeralCosmosContainer _container;

    protected EphemeralCosmosContainerFixture(
        Database database,
        EphemeralOptions options,
        CosmosContainerOptions cosmosContainerOptions = default)
    {
        _container = new(
            database,
            options ?? EphemeralOptions.Default,
            cosmosContainerOptions ?? CosmosContainerOptions.Default);
    }

    async Task IAsyncLifetime.InitializeAsync() =>
        await _container.GetAsync();

    async Task IAsyncLifetime.DisposeAsync() =>
        await ((IAsyncDisposable)this).DisposeAsync();

    ValueTask IAsyncDisposable.DisposeAsync() =>
        _container.DisposeAsync();
}
