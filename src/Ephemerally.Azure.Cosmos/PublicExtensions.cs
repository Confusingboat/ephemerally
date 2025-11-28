using Ephemerally.Azure.Cosmos;
using Microsoft.Azure.Cosmos;

// ReSharper disable once CheckNamespace
namespace Ephemerally;

public static class PublicExtensions
{
    private const string DefaultPartitionKeyPath = "/id";

    public static EphemeralCosmosDatabase ToEphemeral(this Database database, EphemeralOptions options = null) =>
        new(new CosmosDatabaseEphemeral(database, options));

    public static EphemeralCosmosContainer ToEphemeral(this Container container, EphemeralOptions options = null) =>
        new(new CosmosContainerEphemeral(container, options));

    public static async Task<EphemeralCosmosDatabase> CreateEphemeralDatabaseAsync(
        this CosmosClient client,
        EphemeralCreationOptions options = null)
    {
        var metadata = options.OrDefault().GetNewNamedMetadata();
        var response = await client.CreateDatabaseIfNotExistsAsync(metadata.FullName).ConfigureAwait(false);
        return client.GetDatabase(response.Resource.Id).ToEphemeral(options);
    }

    public static async Task<EphemeralCosmosContainer> CreateEphemeralContainerAsync(
        this Database database,
        EphemeralCreationOptions options = null,
        ContainerProperties containerProperties = null,
        ThroughputProperties throughputProperties = null)
    {
        var metadata = options.OrDefault().GetNewNamedMetadata();
        containerProperties ??= new();
        containerProperties.Id ??= metadata.FullName;
        containerProperties.PartitionKeyPath ??= DefaultPartitionKeyPath;
        var response = await database.CreateContainerIfNotExistsAsync(containerProperties, throughputProperties).ConfigureAwait(false);
        return database.GetContainer(response.Resource.Id).ToEphemeral(options);
    }

    public static IEphemeralMetadata GetEphemeralMetadata(this DatabaseProperties container) =>
        container.Id.GetNamedMetadata();
    
    public static IEphemeralMetadata GetEphemeralMetadata(this Database container) =>
        container.Id.GetNamedMetadata();

    public static IEphemeralMetadata GetEphemeralMetadata(this ContainerProperties container) =>
        container.Id.GetNamedMetadata();

    public static IEphemeralMetadata GetEphemeralMetadata(this Container container) =>
        container.Id.GetNamedMetadata();
}