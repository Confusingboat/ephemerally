namespace EphemeralDb.Cosmos;

public record CosmosContainerOptions
{
    public string PartitionKeyPath { get; init; }

    public static CosmosContainerOptions Default => new()
    {
        PartitionKeyPath = "/id"
    };
}