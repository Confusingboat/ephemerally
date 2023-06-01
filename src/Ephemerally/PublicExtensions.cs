namespace Ephemerally;

public static class PublicExtensions
{
    //public static EphemeralCreationOptions OrDefault(this EphemeralCreationOptions options) =>
    //    options ?? EphemeralCreationOptions.Default;

    public static T OrDefault<T>(this T options) where T : EphemeralOptions, new() =>
        options ?? new T();

    public static EphemeralMetadata GetNewMetadata(this EphemeralCreationOptions options) =>
        EphemeralMetadata.New(options.Name, options.GetExpiration(DateTimeOffset.UtcNow));

    public static bool IsExpired(this EphemeralMetadata metadata) =>
        IsExpired(metadata, DateTimeOffset.UtcNow);

    internal static bool IsExpired(this EphemeralMetadata metadata, DateTimeOffset now) =>
        metadata.Expiration.HasValue && metadata.Expiration.Value <= now;

    public static EphemeralMetadata GetContainerMetadata(this string fullName) =>
        EphemeralMetadata.New(fullName);
}