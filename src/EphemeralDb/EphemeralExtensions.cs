using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EphemeralDb;

public static class EphemeralExtensions
{
    public static bool IsExpired(this EphemeralMetadata metadata) =>
        metadata.Expiration.HasValue && metadata.Expiration.Value < DateTimeOffset.UtcNow;
}