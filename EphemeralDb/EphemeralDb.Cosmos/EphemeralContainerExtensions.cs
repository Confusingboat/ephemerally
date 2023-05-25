using Microsoft.Azure.Cosmos;

namespace EphemeralDb.Cosmos;

public static class EphemeralContainerExtensions
{
    public static bool IsExpired(this EphemeralMetadata metadata) =>
        metadata.Expiration.HasValue && metadata.Expiration.Value < DateTimeOffset.UtcNow;


    public static EphemeralMetadata GetContainerMetadata(this ContainerProperties container) =>
        container.Id.GetContainerMetadata();
    public static bool IsExpired(this ContainerProperties container) =>
        container.Id.GetContainerMetadata().IsExpired();


    public static EphemeralMetadata GetContainerMetadata(this Container container) =>
        container.Id.GetContainerMetadata();
    public static bool IsExpired(this Container container) =>
        container.Id.GetContainerMetadata().IsExpired();


    public static async Task CleanupContainersAsync(this Database database)
    {
        var exceptions = new List<Exception>();

        var containers = database
            .GetContainerQueryIterator<ContainerProperties>()
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