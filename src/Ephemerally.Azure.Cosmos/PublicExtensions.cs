using Ephemerally.Azure.Cosmos;
using Microsoft.Azure.Cosmos;

// ReSharper disable once CheckNamespace
namespace Ephemerally;

public static class PublicExtensions
{
    public static EphemeralCosmosDatabase ToEphemeral(this Database database, EphemeralOptions options = default) =>
        new(new CosmosDatabaseEphemeral(database, options));

    public static EphemeralCosmosContainer ToEphemeral(this Container container, EphemeralOptions options = default) =>
        new(new CosmosContainerEphemeral(container, options));

    public static async Task<EphemeralCosmosDatabase> CreateEphemeralDatabaseAsync(
        this CosmosClient client,
        EphemeralCreationOptions options = default)
    {
        var metadata = options.OrDefault().GetNewMetadata();
        var response = await client.CreateDatabaseIfNotExistsAsync(metadata.FullName).ConfigureAwait(false);
        return client.GetDatabase(response.Resource.Id).ToEphemeral(options);
    }

    public static async Task<EphemeralCosmosContainer> CreateEphemeralContainerAsync(
        this Database database,
        EphemeralCreationOptions options = default,
        ContainerProperties containerProperties = default,
        ThroughputProperties throughputProperties = default)
    {
        var metadata = options.OrDefault().GetNewMetadata();
        containerProperties ??= new ContainerProperties(metadata.FullName, "/id");
        var response = await database.CreateContainerIfNotExistsAsync(containerProperties, throughputProperties).ConfigureAwait(false);
        return database.GetContainer(response.Resource.Id).ToEphemeral(options);
    }

    public static EphemeralMetadata GetEphemeralMetadata(this DatabaseProperties container) =>
        container.Id.GetContainerMetadata();
    
    public static EphemeralMetadata GetEphemeralMetadata(this Database container) =>
        container.Id.GetContainerMetadata();

    public static EphemeralMetadata GetEphemeralMetadata(this ContainerProperties container) =>
        container.Id.GetContainerMetadata();

    public static EphemeralMetadata GetEphemeralMetadata(this Container container) =>
        container.Id.GetContainerMetadata();
}