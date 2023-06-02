using Microsoft.Azure.Cosmos;
using System.Diagnostics;

namespace Ephemerally.Azure.Cosmos;

internal static class InternalExtensions
{
    internal static T OrDefault<T>(this T options) where T : EphemeralOptions, new() =>
        options ?? new T();

    internal static async Task<bool> ExistsAsync(this Database database) =>
        await database.Client.DatabaseExistsAsync(database.Id).ConfigureAwait(false);

    internal static async Task<bool> DatabaseExistsAsync(this CosmosClient client, string databaseId)
    {
        var query = new QueryDefinition("select * from c where c.id = @databaseId")
            .WithParameter("@databaseId", databaseId);

        return await client.GetDatabaseQueryIterator<object>(query)
            .ToAsyncEnumerable()
            .AnyAsync(x => x.Resource.Any()).ConfigureAwait(false);
    }

    internal static async Task<bool> ExistsAsync(this Container container) =>
        await container.Database.ContainerExistsAsync(container.Id).ConfigureAwait(false);
    internal static async Task<bool> ContainerExistsAsync(this Database database, string containerId)
    {
        var query = new QueryDefinition("select * from c where c.id = @containerId")
            .WithParameter("@containerId", containerId);

        return await database.GetContainerQueryIterator<object>(query)
            .ToAsyncEnumerable()
            .AnyAsync(x => x.Resource.Any()).ConfigureAwait(false);
    }

    internal static async IAsyncEnumerable<FeedResponse<T>> ToAsyncEnumerable<T>(this FeedIterator<T> iterator)
    {
        while (iterator.HasMoreResults)
        {
            yield return await iterator.ReadNextAsync().ConfigureAwait(false);
        }
    }

    internal static IAsyncEnumerable<T> SelectResources<T>(this IAsyncEnumerable<FeedResponse<T>> enumerable) =>
        enumerable.SelectMany(x => x.Resource.ToAsyncEnumerable());

    internal static async Task<bool> TryDeleteDatabaseAsync(this CosmosClient client, string databaseId)
    {
        try
        {
            await client.GetDatabase(databaseId).DeleteAsync().ConfigureAwait(false);
            return true;
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.ToString());
            return false;
        }
    }

    internal static bool IsExpired(this DatabaseProperties container) =>
        container.Id.GetContainerMetadata().IsExpired();

    internal static bool IsExpired(this Database container) =>
        container.Id.GetContainerMetadata().IsExpired();

    internal static bool IsExpired(this ContainerProperties container) =>
        container.Id.GetContainerMetadata().IsExpired();

    internal static bool IsExpired(this Container container) =>
        container.Id.GetContainerMetadata().IsExpired();

    internal static async Task<IEnumerable<DatabaseProperties>> GetExpiredDatabasesAsync(this CosmosClient client)
    {
        using var iterator = client.GetDatabaseQueryIterator<DatabaseProperties>();
        return await iterator.GetExpiredDatabasesAsync().ToListAsync().ConfigureAwait(false);
    }

    internal static IAsyncEnumerable<DatabaseProperties> GetExpiredDatabasesAsync(this FeedIterator<DatabaseProperties> iterator) =>
        iterator
            .ToAsyncEnumerable()
            .SelectResources()
            .Where(IsExpired);

    internal static async Task TryCleanupDatabasesAsync(this CosmosClient client)
    {
        foreach (var db in await client.GetExpiredDatabasesAsync().ConfigureAwait(false))
        {
            await client.TryDeleteDatabaseAsync(db.Id).ConfigureAwait(false);
        }
    }

    internal static async Task<bool> TryDeleteContainerAsync(this Database database, string containerId)
    {
        try
        {
            await database.GetContainer(containerId).DeleteContainerAsync().ConfigureAwait(false);
            return true;
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.ToString());
            return false;
        }
    }

    internal static async Task<IEnumerable<ContainerProperties>> GetExpiredContainersAsync(this Database database)
    {
        using var iterator = database.GetContainerQueryIterator<ContainerProperties>();
        return await iterator.GetExpiredContainersAsync().ToListAsync().ConfigureAwait(false);
    }

    internal static IAsyncEnumerable<ContainerProperties> GetExpiredContainersAsync(this FeedIterator<ContainerProperties> iterator) =>
        iterator
            .ToAsyncEnumerable()
            .SelectResources()
            .Where(IsExpired);

    internal static async Task TryCleanupContainersAsync(this Database database)
    {
        foreach (var container in await database.GetExpiredContainersAsync().ConfigureAwait(false))
        {
            await database.TryDeleteContainerAsync(container.Id).ConfigureAwait(false);
        }
    }

    internal static IAsyncEnumerable<T> OnEach<T>(this IAsyncEnumerable<T> enumerable, Action<T> action) =>
        enumerable.Select(x =>
        {
            action(x);
            return x;
        });
}