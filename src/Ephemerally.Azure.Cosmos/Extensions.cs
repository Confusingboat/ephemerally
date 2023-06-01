using Microsoft.Azure.Cosmos;

namespace Ephemerally.Azure.Cosmos;

public static class Extensions
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
        var response = await client.CreateDatabaseIfNotExistsAsync(metadata.FullName);
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
        var response = await database.CreateContainerIfNotExistsAsync(containerProperties, throughputProperties);
        return database.GetContainer(response.Resource.Id).ToEphemeral(options);
    }

    public static async Task<bool> ExistsAsync(this Database database) =>
        await database.Client.DatabaseExistsAsync(database.Id);

    public static async Task<bool> DatabaseExistsAsync(this CosmosClient client, string databaseId)
    {
        var query = new QueryDefinition("select * from c where c.id = @databaseId")
            .WithParameter("@databaseId", databaseId);

        return await client.GetDatabaseQueryIterator<object>(query)
            .ToAsyncEnumerable()
            .AnyAsync(x => x.Resource.Any());
    }

    public static async Task<bool> ExistsAsync(this Container container) => 
        await container.Database.ContainerExistsAsync(container.Id);
    public static async Task<bool> ContainerExistsAsync(this Database database, string containerId)
    {
        var query = new QueryDefinition("select * from c where c.id = @containerId")
            .WithParameter("@containerId", containerId);

        return await database.GetContainerQueryIterator<object>(query)
            .ToAsyncEnumerable()
            .AnyAsync(x => x.Resource.Any());
    }

    public static async IAsyncEnumerable<FeedResponse<T>> ToAsyncEnumerable<T>(this FeedIterator<T> iterator)
    {
        while (iterator.HasMoreResults)
        {
            yield return await iterator.ReadNextAsync();
        }
    }

    public static IAsyncEnumerable<T> SelectResources<T>(this IAsyncEnumerable<FeedResponse<T>> enumerable) =>
        enumerable.SelectMany(x => x.Resource.ToAsyncEnumerable());

}