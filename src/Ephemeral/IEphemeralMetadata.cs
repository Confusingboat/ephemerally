namespace Ephemeral;

public interface IEphemeralMetadata
{
    string FullName { get; }
    string NamePart { get; }
    string Nonce { get; }
    bool IsEphemeral { get; }
    DateTimeOffset? Expiration { get; }
}