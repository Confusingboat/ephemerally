using Microsoft.Azure.Cosmos;

namespace Ephemerally.Azure.Cosmos.Xunit;

public class DefaultCosmosEmulatorClientFixture : CosmosClientFixture
{
    protected override Task<CosmosClient> CreateSubjectAsync() => Task.FromResult(CosmosEmulator.GetClient());
}

public class DefaultEphemeralCosmosDatabaseFixture()
    : EphemeralCosmosDatabaseFixture(new DefaultCosmosEmulatorClientFixture());

public class DefaultEphemeralCosmosContainerFixture()
    : EphemeralCosmosContainerFixture(new DefaultEphemeralCosmosDatabaseFixture());