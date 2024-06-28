using System.Collections;

namespace Ephemerally.EnvironmentVariables;

internal static class EnvVarOwners
{
    public static object SyncRoot => ((ICollection)Entries).SyncRoot;

    public static Dictionary<string, VarOwnerEntry> Entries { get; } = new();

    public static void CleanupAll()
    {
        lock (SyncRoot)
        {
            var now = DateTimeOffset.UtcNow;
            var expired = Entries
                .Where(x => x.Value.Expiration < now)
                .Select(x => x.Key)
                .ToList();

            foreach (var key in expired)
            {
                Entries.Remove(key);
            }
        }
    }
}

internal readonly record struct VarOwnerEntry(Guid OwnerId, string Variable, DateTimeOffset Expiration);