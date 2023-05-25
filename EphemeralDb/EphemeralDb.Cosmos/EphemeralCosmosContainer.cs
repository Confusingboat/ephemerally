using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EphemeralDb.Cosmos;

public class EphemeralCosmosContainer : Ephemeral<Container>
{
    private readonly Database _database;
    private readonly CosmosContainerOptions _cosmosContainerOptions;

    public EphemeralCosmosContainer(
        Database database,
        EphemeralOptions options,
        CosmosContainerOptions cosmosContainerOptions) :
        base(options)
    {
        _database = database;
        _cosmosContainerOptions = cosmosContainerOptions;
    }
    protected override async Task<Container> EnsureExistsAsync()
    {
        var containerProperties = new ContainerProperties(FullName, _cosmosContainerOptions.PartitionKeyPath);
        return await _database.CreateContainerIfNotExistsAsync(containerProperties);
    }

    protected override Task CleanupSelfAsync() =>
        _database.GetContainer(FullName).DeleteContainerAsync();

    protected override Task CleanupAllAsync() =>
        _database.CleanupContainersAsync();
}