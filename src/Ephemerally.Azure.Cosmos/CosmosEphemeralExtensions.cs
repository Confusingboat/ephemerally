using System.Diagnostics;
using Microsoft.Azure.Cosmos;

namespace Ephemerally.Azure.Cosmos;

public static class CosmosEphemeralExtensions
{
    public static EphemeralMetadata GetContainerMetadata(this DatabaseProperties container) =>
        container.Id.GetContainerMetadata();
    public static bool IsExpired(this DatabaseProperties container) =>
        container.Id.GetContainerMetadata().IsExpired();


    public static EphemeralMetadata GetContainerMetadata(this Database container) =>
        container.Id.GetContainerMetadata();
    public static bool IsExpired(this Database container) =>
        container.Id.GetContainerMetadata().IsExpired();


    public static EphemeralMetadata GetContainerMetadata(this ContainerProperties container) =>
        container.Id.GetContainerMetadata();
    public static bool IsExpired(this ContainerProperties container) =>
        container.Id.GetContainerMetadata().IsExpired();


    public static EphemeralMetadata GetContainerMetadata(this Container container) =>
        container.Id.GetContainerMetadata();
    public static bool IsExpired(this Container container) =>
        container.Id.GetContainerMetadata().IsExpired();


    public static async Task<bool> TryDeleteDatabaseAsync(this CosmosClient client, string databaseId)
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
    public static async Task TryCleanupDatabasesAsync(this CosmosClient client)
    {
        foreach (var db in await client.GetExpiredDatabasesAsync().ConfigureAwait(false))
        {
            await client.TryDeleteDatabaseAsync(db.Id).ConfigureAwait(false);
        }
    }

    public static async Task<bool> TryDeleteContainerAsync(this Database database, string containerId)
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

    public static async Task TryCleanupContainersAsync(this Database database)
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