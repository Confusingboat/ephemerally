namespace Ephemeral;

public interface IEphemeralMetadata
{
    string FullName { get; }
    string NamePart { get; init; }
    string Nonce { get; init; }
    bool IsEphemeral { get; init; }
    DateTimeOffset? Expiration { get; init; }
}