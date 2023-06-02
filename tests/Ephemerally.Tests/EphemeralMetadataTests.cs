namespace Ephemerally.Tests;

public class EphemeralMetadataTests
{
    [TestCase(1684991963, "ABCDEF", "Test", ExpectedResult = "E_1684991963_ABCDEF_Test")]
    public string GetFullName_Tests(long expirationTimestamp, string nonce, string name) =>
        EphemeralMetadata.GetFullName(expirationTimestamp, nonce, name);

    [TestCase(1684991962, ExpectedResult = false)]
    [TestCase(1684991963, ExpectedResult = true)]
    [TestCase(1684991964, ExpectedResult = true)]
    public bool IsExpired_as_of_1684991963(long now)
    {
        var metadata = EphemeralMetadata.New("E_1684991963_ABCDEF_Test") with
        {
            Expiration = DateTimeOffset.FromUnixTimeSeconds(1684991963),
            NamePart = "Test",
            Nonce = "ABCDEF"
        };

        return metadata.IsExpiredAsOf(DateTimeOffset.FromUnixTimeSeconds(now));
    }
}