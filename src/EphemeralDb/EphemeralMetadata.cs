namespace EphemeralDb;

public record EphemeralMetadata : IEphemeralMetadata
{
    private const string PrefixValue = "E";

    public string FullName { get; }
    public string NamePart { get; init; }
    public string Nonce { get; init; }
    public bool IsEphemeral { get; init; }
    public DateTimeOffset? Expiration { get; init; }

    internal EphemeralMetadata(string fullName)
    {
        FullName = fullName;
    }

    public override string ToString() => FullName;

    public static EphemeralMetadata FromString(string fullName) =>
        fullName.Split('_') is
        [PrefixValue, var ts, var nonce, var friendlyName] &&
        long.TryParse(ts, out var timestamp)
            ? new EphemeralMetadata(fullName)
            {
                Expiration = DateTimeOffset.FromUnixTimeSeconds(timestamp),
                NamePart = friendlyName,
                Nonce = nonce,
                IsEphemeral = true
            }
            : new(fullName);

    public static string GetFullName(
        long expirationTimestampSeconds,
        string nonce,
        string name) =>
        $"{PrefixValue}_{expirationTimestampSeconds}_{nonce}_{name}";

    public static string GetFullName(
        string name,
        int? containerLifetimeSeconds)
    {
        if (!containerLifetimeSeconds.HasValue) return name;

        var expirationTimestamp = DateTimeOffset.UtcNow.AddSeconds(containerLifetimeSeconds.Value).ToUnixTimeSeconds();
        var nonce = Guid.NewGuid().ToString()[..6];

        return GetFullName(expirationTimestamp, nonce, name);
    }
}