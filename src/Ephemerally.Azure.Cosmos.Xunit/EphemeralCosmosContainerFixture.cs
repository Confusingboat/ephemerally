using System.Diagnostics.CodeAnalysis;

namespace Ephemerally.Azure.Cosmos.Xunit;

[SuppressMessage("ReSharper", "UseConfigureAwaitFalse")]
public class EphemeralCosmosContainerFixture : EphemeralCosmosDatabaseFixture
{
    private readonly Lazy<Task<EphemeralCosmosContainer>> _container;

    public EphemeralCosmosContainer Container => _container.Value.Result;

    protected Task<EphemeralCosmosContainer> GetContainer() => _container.Value;

    public EphemeralCosmosContainerFixture()
    {
        _container = new(CreateContainerAsync);
    }

    protected virtual async Task<EphemeralCosmosContainer> CreateContainerAsync()
    {
        var db = await GetDatabase();
        return await db.CreateEphemeralContainerAsync();
    }

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        await _container.Value;
    }

    public override async Task DisposeAsync()
    {
        if (!_container.IsValueCreated) return;

        await IgnoreSocketException(async () =>
        {
            var container = await GetContainer();
            await container.DisposeAsync();
        });

        await base.DisposeAsync();
    }
}