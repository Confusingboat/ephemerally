using Microsoft.Azure.Cosmos;

namespace Ephemerally.Azure.Cosmos;

public class CosmosDatabaseEphemeral : Ephemeral<Database>
{
    public CosmosDatabaseEphemeral(
        Database database,
        EphemeralOptions options = default)
        : base(database, x => x.Id, options.OrDefault())
    { }

    protected override Task CleanupSelfAsync(string fullName) =>
        Value.Client.TryDeleteDatabaseAsync(fullName);

    protected override Task CleanupAllAsync() =>
        Value.Client.TryCleanupDatabasesAsync();
}