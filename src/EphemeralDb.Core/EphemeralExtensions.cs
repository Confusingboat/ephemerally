using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EphemeralDb;

public static class EphemeralExtensions
{
    public static bool IsExpired(this EphemeralMetadata metadata) =>
        IsExpired(metadata, DateTimeOffset.UtcNow);

    internal static bool IsExpired(this EphemeralMetadata metadata, DateTimeOffset now) =>
        metadata.Expiration.HasValue && metadata.Expiration.Value <= now;

    public static EphemeralMetadata GetContainerMetadata(this string fullName) =>
        EphemeralMetadata.FromString(fullName);
}