namespace Ephemerally;

public interface IEphemeralMetadata
{
    DateTimeOffset? Expiration { get; }
}