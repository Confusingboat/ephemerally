using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EphemeralDb.Cosmos;

public class EphemeralCosmosContainer : Ephemeral<Container>
{
    private readonly Func<ValueTask<Database>> _getDatabase;
    private readonly CosmosContainerOptions _cosmosContainerOptions;

    public EphemeralCosmosContainer(
        Func<ValueTask<Database>> getDatabase,
        EphemeralOptions options = default,
        CosmosContainerOptions cosmosContainerOptions = default) :
        base(options.OrDefault())
    {
        _getDatabase = getDatabase;
        _cosmosContainerOptions = cosmosContainerOptions ?? CosmosContainerOptions.Default;
    }

    public EphemeralCosmosContainer(
        Database database,
        EphemeralOptions options = default,
        CosmosContainerOptions cosmosContainerOptions = default) :
        this(() => new ValueTask<Database>(database), options, cosmosContainerOptions)
    { }

    protected override async Task<Container> EnsureExistsAsync(string fullName)
    {
        var containerProperties = new ContainerProperties(fullName, _cosmosContainerOptions.PartitionKeyPath);
        return await (await _getDatabase()).CreateContainerIfNotExistsAsync(containerProperties);
    }

    protected override async Task CleanupSelfAsync(string fullName) =>
        await (await _getDatabase()).TryDeleteContainerAsync(fullName);

    protected override async Task CleanupAllAsync() =>
        await (await _getDatabase()).TryCleanupContainersAsync();
}