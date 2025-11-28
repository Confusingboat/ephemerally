using Microsoft.Azure.Cosmos;

namespace Ephemerally.Azure.Cosmos;

public class CosmosContainerEphemeral : Ephemeral<Container>
{
    public CosmosContainerEphemeral(
        Container container,
        EphemeralOptions options = null) :
        base(container, options.OrDefault())
    { }

    protected override Task CleanupSelfAsync() =>
        Value.Database.TryDeleteContainerAsync(Value.Id);

    protected override Task CleanupAllAsync() =>
        Value.Database.TryCleanupContainersAsync();
}