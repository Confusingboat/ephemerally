namespace Ephemerally;

public static class PublicExtensions
{
    public static IEphemeralMetadata GetNewMetadata(this EphemeralCreationOptions options) =>
        EphemeralMetadata.New(options.Name, options.GetExpiration(DateTimeOffset.UtcNow));

    public static bool IsExpired(this IEphemeralMetadata metadata) =>
        IsExpiredAsOf(metadata, DateTimeOffset.UtcNow);

    public static bool IsExpiredAsOf(this IEphemeralMetadata metadata, DateTimeOffset now) =>
        metadata.Expiration.HasValue && metadata.Expiration.Value <= now;

    public static IEphemeralMetadata GetContainerMetadata(this string fullName) =>
        EphemeralMetadata.Parse(fullName);

    public static IEphemeral<T> ToEphemeral<T>(this T value,
        Func<Task> cleanupSelfAsync) where T : class => new SingleEphemeral<T>(value, cleanupSelfAsync);
}