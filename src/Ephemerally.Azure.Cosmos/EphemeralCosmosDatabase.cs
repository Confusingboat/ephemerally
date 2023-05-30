using Microsoft.Azure.Cosmos;

namespace Ephemerally.Azure.Cosmos;

public class EphemeralCosmosDatabase : Ephemeral<Database>
{
    private readonly CosmosClient _cosmosClient;

    public EphemeralCosmosDatabase(CosmosClient cosmosClient, EphemeralOptions options = default) : base(options.OrDefault())
    {
        _cosmosClient = cosmosClient;
    }

    protected override async Task<Database> EnsureExistsAsync(string fullName) =>
        await _cosmosClient.CreateDatabaseIfNotExistsAsync(fullName);

    protected override Task CleanupSelfAsync(string fullName) =>
        _cosmosClient.TryDeleteDatabaseAsync(fullName);

    protected override async Task CleanupAllAsync() =>
        await _cosmosClient.TryCleanupDatabasesAsync();
}