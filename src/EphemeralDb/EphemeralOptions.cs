using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Versioning;
using System.Text;
using System.Threading.Tasks;

namespace EphemeralDb;

public record EphemeralOptions
{
    public TimeSpan ContainerLifetime { get; init; } = TimeSpan.FromMinutes(1);
    public string Name { get; init; } = "Ephemeral";
    public CleanupBehavior CleanupBehavior { get; init; } = CleanupBehavior.SelfAndExpired;
    public CreationCachingBehavior CreationCachingBehavior { get; init; } = CreationCachingBehavior.Cache;

    public static EphemeralOptions Default => new();
}