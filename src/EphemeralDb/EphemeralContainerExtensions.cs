namespace EphemeralDb;

public static class EphemeralContainerExtensions
{
    public static EphemeralMetadata GetContainerMetadata(this string containerId) =>
        containerId.Split('_') is
            ["Exp", var ts, var nonce, var friendlyName]
            ? new EphemeralMetadata(containerId)
            {
                Expiration = DateTimeOffset.FromUnixTimeSeconds(long.Parse(ts)),
                NamePart = friendlyName,
                Nonce = nonce,
                IsEphemeral = true
            }
            : new(containerId);
}