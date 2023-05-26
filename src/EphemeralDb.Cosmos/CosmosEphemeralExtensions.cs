using System.Net;
using Microsoft.Azure.Cosmos;

namespace EphemeralDb.Cosmos;

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
            await client.GetDatabase(databaseId).DeleteAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    internal static IAsyncEnumerable<DatabaseProperties> GetExpiredDatabases(this FeedIterator<DatabaseProperties> iterator) =>
        iterator
            .ToAsyncEnumerable()
            .SelectResources()
            .Where(IsExpired);
    public static async Task TryCleanupDatabasesAsync(this CosmosClient client)
    {
        using var iterator = client.GetDatabaseQueryIterator<DatabaseProperties>();
        await iterator.GetExpiredDatabases().ForEachAwaitAsync(x => client.TryDeleteDatabaseAsync(x.Id));
    }

    public static async Task<bool> TryDeleteContainerAsync(this Database database, string containerId)
    {
        try
        {
            await database.GetContainer(containerId).DeleteContainerAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    internal static IAsyncEnumerable<ContainerProperties> GetExpiredContainers(this FeedIterator<ContainerProperties> iterator) =>
        iterator
            .ToAsyncEnumerable()
            .SelectResources()
            .Where(IsExpired);

    public static async Task TryCleanupContainersAsync(this Database database)
    {
        using var iterator = database.GetContainerQueryIterator<ContainerProperties>();
        await iterator.GetExpiredContainers().ForEachAwaitAsync(x => database.TryDeleteContainerAsync(x.Id));
    }
}