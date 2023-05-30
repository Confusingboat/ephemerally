namespace Ephemerally;

public record EphemeralOptions
{
    public TimeSpan? Lifetime { get; }
    public DateTimeOffset? Expiration { get; }
    public string Name { get; init; } = "Ephemeral";
    public CleanupBehavior CleanupBehavior { get; init; } = CleanupBehavior.SelfAndExpired;
    public CreationCachingBehavior CreationCachingBehavior { get; init; } = CreationCachingBehavior.Cache;

    public EphemeralOptions() : this(TimeSpan.FromMinutes(1)) { }
    public EphemeralOptions(TimeSpan lifetime)
    {
        Lifetime = lifetime;
    }

    public EphemeralOptions(DateTimeOffset expiration)
    {
        Expiration = expiration;
    }

    public DateTimeOffset GetExpiration(DateTimeOffset fromTime) =>
        Expiration ?? fromTime + Lifetime!.Value;

    public static EphemeralOptions Default => new();
}