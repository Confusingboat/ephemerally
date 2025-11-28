namespace Ephemerally;

public static class PublicExtensions
{
    public static NamedEphemeralMetadata GetNewNamedMetadata(this EphemeralCreationOptions options) =>
        NamedEphemeralMetadata.New(options.Name, options.GetExpiration(DateTimeOffset.UtcNow));

    public static bool IsExpired(this IEphemeralMetadata metadata) =>
        IsExpiredAsOf(metadata, DateTimeOffset.UtcNow);

    public static bool IsExpiredAsOf(this IEphemeralMetadata metadata, DateTimeOffset now) =>
        metadata.Expiration.HasValue && metadata.Expiration.Value <= now;

    public static NamedEphemeralMetadata GetNamedMetadata(this string fullName) =>
        NamedEphemeralMetadata.Parse(fullName);

    public static IEphemeral<T> ToEphemeral<T>(this T value,
        Func<Task> cleanupSelfAsync) where T : class => new SingleEphemeral<T>(value, cleanupSelfAsync);
}