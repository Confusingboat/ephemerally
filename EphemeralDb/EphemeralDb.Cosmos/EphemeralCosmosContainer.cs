using Microsoft.Azure.Cosmos;

namespace EphemeralDb.Cosmos;

public static class EphemeralContainerExtensions
{
    public static bool IsExpired(this ContainerMetadata metadata) =>
        metadata.Expiration.HasValue && metadata.Expiration.Value < DateTimeOffset.UtcNow;


    public static ContainerMetadata GetContainerMetadata(this ContainerProperties container) =>
        container.Id.GetContainerMetadata();
    public static bool IsExpired(this ContainerProperties container) =>
        container.Id.GetContainerMetadata().IsExpired();


    public static ContainerMetadata GetContainerMetadata(this Container container) =>
        container.Id.GetContainerMetadata();
    public static bool IsExpired(this Container container) =>
        container.Id.GetContainerMetadata().IsExpired();


    internal static ContainerMetadata GetContainerMetadata(this string containerId) =>
        containerId.Split('_') is
            ["Exp", var ts, var nonce, var friendlyName]
            ? new ContainerMetadata(containerId)
            {
                Expiration = DateTimeOffset.FromUnixTimeSeconds(long.Parse(ts)),
                Name = friendlyName,
                Nonce = nonce,
                IsEphemeral = true
            }
            : new(containerId);

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

public record ContainerMetadata
{
    public string Id { get; }
    public string Name { get; init; }
    public string Nonce { get; init; }
    public bool IsEphemeral { get; init; }
    public DateTimeOffset? Expiration { get; init; }

    internal ContainerMetadata(string containerId)
    {
        Id = containerId;
    }

    internal static string GetContainerId(
        string containerName,
        int? containerLifetimeSeconds)
    {
        if (!containerLifetimeSeconds.HasValue) return containerName;

        var expirationTimestamp = DateTimeOffset.UtcNow.AddSeconds(containerLifetimeSeconds.Value).ToUnixTimeSeconds();
        var nonce = Guid.NewGuid().ToString()[..6];

        return $"E_{expirationTimestamp}_{nonce}_{containerName}";
    }
}

public enum CleanupBehavior
{
    NoCleanup = 0,
    SelfOnly = 1,
    SelfAndExpired = 2
}

public enum CreationCachingBehavior
{
    NoCache = 0,
    Cache = 1
}

public class EphemeralCosmosContainerOptions
{
    public string PartitionKeyPath { get; init; }
    public int ContainerLifetimeSeconds { get; init; }
    public string ContainerName { get; init; }
    public CleanupBehavior CleanupBehavior { get; init; }
    public CreationCachingBehavior CreationCachingBehavior { get; init; }

    public static EphemeralCosmosContainerOptions Default => new()
    {
        PartitionKeyPath = "/id",
        ContainerLifetimeSeconds = 60,
        ContainerName = "Ephemeral",
        CleanupBehavior = CleanupBehavior.SelfAndExpired,
        CreationCachingBehavior = CreationCachingBehavior.Cache
    };
}

public class EphemeralCosmosContainer : IAsyncDisposable
{
    private readonly Database _database;
    private readonly string _containerId;

    private readonly EphemeralCosmosContainerOptions _options;

    private readonly Lazy<Task<Container>> _container;

    public EphemeralCosmosContainer(Database database,
        int containerLifetimeSeconds = 60,
        string partitionKeyPath = "/id",
        string containerName = "Ephemeral",
        CleanupBehavior cleanupBehavior = CleanupBehavior.SelfAndExpired,
        CreationCachingBehavior creationCachingBehavior = CreationCachingBehavior.Cache)
        : this(database, new EphemeralCosmosContainerOptions
        {
            ContainerLifetimeSeconds = containerLifetimeSeconds,
            PartitionKeyPath = partitionKeyPath,
            ContainerName = containerName,
            CleanupBehavior = cleanupBehavior,
            CreationCachingBehavior = creationCachingBehavior
        })
    {

    }

    public EphemeralCosmosContainer(Database database, EphemeralCosmosContainerOptions options)
    {
        _database = database;
        _options = options;
        _containerId = ContainerMetadata.GetContainerId(options.ContainerName, options.ContainerLifetimeSeconds);
        _container = new Lazy<Task<Container>>(EnsureContainerExistsAsync);
    }

    public EphemeralCosmosContainerOptions Options => _options;

    private async Task<Container> EnsureContainerExistsAsync()
    {
        var containerProperties = new ContainerProperties(_containerId, _options.PartitionKeyPath);
        return await _database.CreateContainerIfNotExistsAsync(containerProperties);
    }

    public async Task<Container> GetContainerAsync()
    {
        if (_options.CreationCachingBehavior == CreationCachingBehavior.Cache)
        {
            return await _container.Value;
        }

        return await EnsureContainerExistsAsync();
    }

    private Task CleanupSelfAsync() =>
        _database.GetContainer(_containerId).DeleteContainerAsync();

    public async ValueTask DisposeAsync()
    {
        if (_options.CleanupBehavior == CleanupBehavior.NoCleanup) return;

        await CleanupSelfAsync();
        if (_options.CleanupBehavior == CleanupBehavior.SelfOnly) return;

        await _database.CleanupContainersAsync();
    }
}
