using Microsoft.Azure.Cosmos;

namespace EphemeralDb.Cosmos;

public static class Extensions
{
    public static EphemeralCosmosDatabase CreateEphemeralDatabaseAsync(
        this CosmosClient client,
        EphemeralOptions options = default) =>
        new(client, options);

    public static EphemeralCosmosContainer CreateEphemeralContainerAsync(
        this Database database,
        EphemeralOptions options = default,
        CosmosContainerOptions cosmosContainerOptions = default) =>
        new(database, options, cosmosContainerOptions);

    public static EphemeralCosmosContainer CreateEphemeralContainerAsync(
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