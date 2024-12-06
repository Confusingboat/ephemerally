using Microsoft.Azure.Cosmos;

namespace Ephemerally.Azure.Cosmos.Xunit;

public class CosmosEmulatorClientFixture(Action<CosmosClientOptions> configureOptions) : CosmosClientFixture
{
    // ReSharper disable once ReplaceWithPrimaryConstructorParameter
    private readonly Action<CosmosClientOptions> _configureOptions = configureOptions;

    public CosmosEmulatorClientFixture() : this(null) { }

    protected override Task<CosmosClient> CreateSubjectAsync() => Task.FromResult(CosmosEmulator.GetClient(_configureOptions));
}