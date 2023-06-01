using Microsoft.Azure.Cosmos;

namespace Ephemerally.Azure.Cosmos;

public class CosmosContainerEphemeral : Ephemeral<Container>
{
    public CosmosContainerEphemeral(
        Container container,
        EphemeralOptions options = default) :
        base(container, x => x.Id, options.OrDefault())
    { }

    protected override Task CleanupSelfAsync(string fullName) =>
        Value.Database.TryDeleteContainerAsync(fullName);

    protected override Task CleanupAllAsync() =>
        Value.Database.TryCleanupContainersAsync();
}