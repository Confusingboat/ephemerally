using System.Diagnostics.CodeAnalysis;

namespace Ephemerally.Azure.Cosmos.Xunit;

[SuppressMessage("ReSharper", "UseConfigureAwaitFalse")]
public class EphemeralCosmosDatabaseFixture : CosmosClientFixture
{
    private readonly Lazy<Task<EphemeralCosmosDatabase>> _database;

    public EphemeralCosmosDatabase Database => _database.Value.Result;

    protected Task<EphemeralCosmosDatabase> GetDatabase() => _database.Value;

    public EphemeralCosmosDatabaseFixture()
    {
        _database = new(CreateDatabaseAsync);
    }

    protected virtual async Task<EphemeralCosmosDatabase> CreateDatabaseAsync()
    {
        var client = await GetClient();
        return await client.CreateEphemeralDatabaseAsync();
    }

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        await _database.Value;
    }

    public override async Task DisposeAsync()
    {
        if (!_database.IsValueCreated) return;

        await IgnoreSocketException(async () =>
        {
            var db = await GetDatabase();
            await db.DisposeAsync();
        });

        await base.DisposeAsync();
    }
}