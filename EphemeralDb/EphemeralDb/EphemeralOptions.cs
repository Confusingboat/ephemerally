using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Versioning;
using System.Text;
using System.Threading.Tasks;

namespace EphemeralDb;

public record EphemeralOptions
{
    public int ContainerLifetimeSeconds { get; init; }
    public string Name { get; init; }
    public CleanupBehavior CleanupBehavior { get; init; }
    public CreationCachingBehavior CreationCachingBehavior { get; init; }

    public static EphemeralOptions Default => new()
    {
        ContainerLifetimeSeconds = 60,
        Name = "Ephemeral",
        CleanupBehavior = CleanupBehavior.SelfAndExpired,
        CreationCachingBehavior = CreationCachingBehavior.Cache
    };
}