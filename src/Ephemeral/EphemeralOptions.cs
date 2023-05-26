namespace Ephemeral;

public record EphemeralOptions
{
    public TimeSpan? ContainerLifetime { get; init; }
    public DateTimeOffset? Expiration { get; init; }
    public string Name { get; init; } = "Ephemeral";
    public CleanupBehavior CleanupBehavior { get; init; } = CleanupBehavior.SelfAndExpired;
    public CreationCachingBehavior CreationCachingBehavior { get; init; } = CreationCachingBehavior.Cache;

    public DateTimeOffset GetExpiration(DateTimeOffset fromTime) =>
        Expiration ?? fromTime + (ContainerLifetime ?? TimeSpan.FromMinutes(1));

    public static EphemeralOptions Default => new();
}