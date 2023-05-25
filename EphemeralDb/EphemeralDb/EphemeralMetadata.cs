namespace EphemeralDb;

public record EphemeralMetadata : IEphemeralMetadata
{
    public string FullName { get; }
    public string NamePart { get; init; }
    public string Nonce { get; init; }
    public bool IsEphemeral { get; init; }
    public DateTimeOffset? Expiration { get; init; }

    internal EphemeralMetadata(string fullName)
    {
        FullName = fullName;
    }

    public static string GetFullName(
        string containerName,
        int? containerLifetimeSeconds)
    {
        if (!containerLifetimeSeconds.HasValue) return containerName;

        var expirationTimestamp = DateTimeOffset.UtcNow.AddSeconds(containerLifetimeSeconds.Value).ToUnixTimeSeconds();
        var nonce = Guid.NewGuid().ToString()[..6];

        return $"E_{expirationTimestamp}_{nonce}_{containerName}";
    }
}