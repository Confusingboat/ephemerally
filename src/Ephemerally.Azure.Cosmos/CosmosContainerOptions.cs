namespace Ephemerally.Azure.Cosmos;

public record CosmosContainerOptions
{
    public string PartitionKeyPath { get; init; }

    public static CosmosContainerOptions Default => new()
    {
        PartitionKeyPath = "/id"
    };
}