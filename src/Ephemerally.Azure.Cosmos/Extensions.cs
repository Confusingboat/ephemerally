using Microsoft.Azure.Cosmos;

namespace Ephemerally.Azure.Cosmos;

public static class Extensions
{
    public static async Task<EphemeralCosmosDatabase> CreateEphemeralDatabaseAsync(
        this CosmosClient client,
        EphemeralOptions options = default)
    {
        var accessor = client.CreateEphemeralDatabaseAccessor(options);
        await accessor.GetAsync();
        return accessor;
    }

    public static EphemeralCosmosDatabase CreateEphemeralDatabaseAccessor(
        this CosmosClient client,
        EphemeralOptions options = default) =>
        new(client, options);

    public static async Task<EphemeralCosmosContainer> CreateEphemeralContainerAsync(
        this Database database,
        EphemeralOptions options = default,
        CosmosContainerOptions cosmosContainerOptions = default)
    {
        var accessor = database.CreateEphemeralContainerAccessor(options, cosmosContainerOptions);
        await accessor.GetAsync();
        return accessor;
    }

    public static EphemeralCosmosContainer CreateEphemeralContainerAccessor(
        this Database database,
        EphemeralOptions options = default,
        CosmosContainerOptions cosmosContainerOptions = default) =>
        new(database, options, cosmosContainerOptions);

    public static EphemeralCosmosContainer CreateEphemeralContainerAccessor(
        this EphemeralCosmosDatabase database,
        EphemeralOptions options = default,
        CosmosContainerOptions cosmosContainerOptions = default) =>
        new(database.GetAsync, options, cosmosContainerOptions);

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