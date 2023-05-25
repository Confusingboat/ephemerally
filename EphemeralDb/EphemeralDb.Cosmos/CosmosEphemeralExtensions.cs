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


    public static async Task CleanupDatabasesAsync(this CosmosClient client)
    {
        var exceptions = new List<Exception>();
        using var iterator = client.GetDatabaseQueryIterator<DatabaseProperties>();
        var containers = iterator
            .ToAsyncEnumerable()
            .SelectResources()
            .Where(IsExpired);

        await foreach (var container in containers)
        {
            try
            {
                await client.GetDatabase(container.Id).DeleteAsync();
            }
            catch (Exception ex)
            {
                exceptions.Add(ex);
            }
        }

        if (exceptions.Any()) throw new AggregateException(exceptions);
    }

    public static async Task CleanupContainersAsync(this Database database)
    {
        var exceptions = new List<Exception>();
        using var iterator = database.GetContainerQueryIterator<ContainerProperties>();
        var containers = iterator
            .ToAsyncEnumerable()
            .SelectResources()
            .Where(IsExpired);

        await foreach (var container in containers)
        {
            try
            {
                await database.GetContainer(container.Id).DeleteContainerAsync();
            }
            catch (Exception ex)
            {
                exceptions.Add(ex);
            }
        }

        if (exceptions.Any()) throw new AggregateException(exceptions);
    }
}