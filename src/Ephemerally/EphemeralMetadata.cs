namespace Ephemerally;

public readonly record struct EphemeralMetadata : IEphemeralMetadata
{
    private const string PrefixValue = "E";

    public string FullName { get; } = string.Empty;
    public string NamePart { get; internal init; }
    public string Nonce { get; internal init; }
    public bool IsEphemeral { get; internal init; }
    public DateTimeOffset? Expiration { get; internal init; }

    /// <summary>
    /// Non-ephemeral constructor
    /// </summary>
    /// <param name="fullName"></param>
    private EphemeralMetadata(string fullName)
    {
        FullName = fullName;
    }

    /// <summary>
    /// Ephemeral constructor
    /// </summary>
    /// <param name="expiration"></param>
    /// <param name="nonce"></param>
    /// <param name="friendlyName"></param>
    /// <param name="fullName"></param>
    private EphemeralMetadata(
        DateTimeOffset expiration,
        string nonce,
        string friendlyName,
        string fullName = default)
    {
        FullName = fullName ?? GetFullName(expiration.ToUnixTimeMilliseconds(), nonce, friendlyName);
        Expiration = expiration;
        NamePart = friendlyName;
        Nonce = nonce;
        IsEphemeral = true;
    }

    public override string ToString() => FullName;

    internal static string GetFullName(
        long expirationTimestamp,
        string nonce,
        string name) =>
        $"{PrefixValue}_{expirationTimestamp}_{nonce}_{name}";

    internal static EphemeralMetadata Parse(string fullName) =>
        fullName.Split('_') is
        [PrefixValue, var ts, var nonce, var friendlyName] &&
        long.TryParse(ts, out var timestamp)
            ? new(DateTimeOffset.FromUnixTimeMilliseconds(timestamp), nonce, friendlyName, fullName)
            : new(fullName);

    internal static EphemeralMetadata New(
        string name,
        DateTimeOffset? expiration)
    {
        if (!expiration.HasValue) return new(name);

        var expirationTimestamp = expiration.Value;
        var nonce = Guid.NewGuid().ToString()[..6];

        return new(expirationTimestamp, nonce, name);
    }

    public static EphemeralMetadata Empty => new(string.Empty);
}