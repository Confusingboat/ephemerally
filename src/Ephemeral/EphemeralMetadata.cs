namespace Ephemeral;

public readonly record struct EphemeralMetadata : IEphemeralMetadata
{
    private const string PrefixValue = "E";

    public string FullName { get; } = string.Empty;
    public string NamePart { get; init; }
    public string Nonce { get; init; }
    public bool IsEphemeral { get; init; }
    public DateTimeOffset? Expiration { get; init; }

    /// <summary>
    /// Non-ephemeral constructor
    /// </summary>
    /// <param name="fullName"></param>
    internal EphemeralMetadata(string fullName)
    {
        FullName = fullName;
    }

    /// <summary>
    /// Ephemeral constructor
    /// </summary>
    /// <param name="expirationTimestampSeconds"></param>
    /// <param name="nonce"></param>
    /// <param name="friendlyName"></param>
    internal EphemeralMetadata(
        long expirationTimestampSeconds,
        string nonce,
        string friendlyName)
    {
        FullName = GetFullName(expirationTimestampSeconds, nonce, friendlyName);
        Expiration = DateTimeOffset.FromUnixTimeSeconds(expirationTimestampSeconds);
        NamePart = friendlyName;
        Nonce = nonce;
        IsEphemeral = true;
    }

    public override string ToString() => FullName;

    public static string GetFullName(
        long expirationTimestampSeconds,
        string nonce,
        string name) =>
        $"{PrefixValue}_{expirationTimestampSeconds}_{nonce}_{name}";

    public static EphemeralMetadata New(string fullName) =>
        fullName.Split('_') is
        [PrefixValue, var ts, var nonce, var friendlyName] &&
        long.TryParse(ts, out var timestamp)
            ? new(timestamp, nonce, friendlyName)
            : new(fullName);

    public static EphemeralMetadata New(
        string name,
        TimeSpan? containerLifetime)
    {
        if (!containerLifetime.HasValue) return new(name);

        var expirationTimestamp = DateTimeOffset.UtcNow.Add(containerLifetime.Value).ToUnixTimeSeconds();
        var nonce = Guid.NewGuid().ToString()[..6];

        return new(expirationTimestamp, nonce, name);
    }

    public static EphemeralMetadata Empty => new(string.Empty);
}