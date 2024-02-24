namespace Ephemerally;

public interface IEphemeralMetadata
{
    string FullName { get; }
    DateTimeOffset? Expiration { get; }
}