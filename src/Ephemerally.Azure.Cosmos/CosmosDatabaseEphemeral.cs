using Microsoft.Azure.Cosmos;

namespace Ephemerally.Azure.Cosmos;

public class CosmosDatabaseEphemeral : Ephemeral<Database>
{
    public CosmosDatabaseEphemeral(
        Database database,
        EphemeralOptions options = default)
        : base(database, options.OrDefault())
    { }

    protected override Task CleanupSelfAsync() =>
        Value.Client.TryDeleteDatabaseAsync(Value.Id);

    protected override Task CleanupAllAsync() =>
        Value.Client.TryCleanupDatabasesAsync();
}