using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ephemerally;

public record EphemeralCreationOptions : EphemeralOptions
{
    public TimeSpan? Lifetime { get; }
    public DateTimeOffset? Expiration { get; }
    public string Name { get; init; } = "Ephemeral";

    public EphemeralCreationOptions() : this(TimeSpan.FromMinutes(1)) { }
    public EphemeralCreationOptions(TimeSpan lifetime)
    {
        Lifetime = lifetime;
    }

    public EphemeralCreationOptions(DateTimeOffset expiration)
    {
        Expiration = expiration;
    }

    public DateTimeOffset GetExpiration(DateTimeOffset fromTime) =>
        Expiration ?? fromTime + Lifetime!.Value;

    public static EphemeralCreationOptions Default => new();
}