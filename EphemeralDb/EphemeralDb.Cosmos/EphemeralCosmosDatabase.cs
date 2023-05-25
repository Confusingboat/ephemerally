using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;

namespace EphemeralDb.Cosmos;

public class EphemeralCosmosDatabase : Ephemeral<Database>
{
    private readonly CosmosClient _cosmosClient;

    public EphemeralCosmosDatabase(CosmosClient cosmosClient, EphemeralOptions options) : base(options)
    {
        _cosmosClient = cosmosClient;
    }

    protected override async Task<Database> EnsureExistsAsync(string fullName) =>
        await _cosmosClient.CreateDatabaseIfNotExistsAsync(fullName);

    protected override Task CleanupSelfAsync(string fullName) =>
        _cosmosClient.GetDatabase(fullName).DeleteAsync();

    protected override async Task CleanupAllAsync() =>
        await _cosmosClient.CleanupDatabasesAsync();
}