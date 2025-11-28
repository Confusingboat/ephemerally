namespace Ephemerally;

public static class PublicExtensions
{
    public static NamedEphemeralMetadata GetNewNamedMetadata(this EphemeralCreationOptions options) =>
        NamedEphemeralMetadata.New(options.Name, options.GetExpiration(DateTimeOffset.UtcNow));

    extension(IEphemeralMetadata metadata)
    {
        public bool IsExpired() =>
            IsExpiredAsOf(metadata, DateTimeOffset.UtcNow);

        public bool IsExpiredAsOf(DateTimeOffset now) =>
            metadata.Expiration.HasValue && metadata.Expiration.Value <= now;
    }

    public static NamedEphemeralMetadata GetNamedMetadata(this string fullName) =>
        NamedEphemeralMetadata.Parse(fullName);

    public static IEphemeral<T> ToEphemeral<T>(this T value,
        Func<Task> cleanupSelfAsync) where T : class => new SingleEphemeral<T>(value, cleanupSelfAsync);
}