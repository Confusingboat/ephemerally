using System.Diagnostics.CodeAnalysis;
using Microsoft.Azure.Cosmos;

namespace Ephemerally.Azure.Cosmos.Xunit;

public class DefaultCosmosEmulatorClientFixture : CosmosClientFixture
{
    protected override Task<CosmosClient> CreateSubjectAsync() => Task.FromResult(CosmosEmulator.GetClient());
}

[SuppressMessage("ReSharper", "UseConfigureAwaitFalse")]
public class DefaultEphemeralCosmosDatabaseFixture()
    : EphemeralCosmosDatabaseFixture(new DefaultCosmosEmulatorClientFixture())
{
    protected override async Task DisposeSubjectAsync()
    {
        try
        {
            await base.DisposeSubjectAsync();
        }
        finally
        {
            await CosmosClientFixture.DisposeAsync();
        }
    }
}

[SuppressMessage("ReSharper", "UseConfigureAwaitFalse")]
public class DefaultEphemeralCosmosContainerFixture()
    : EphemeralCosmosContainerFixture(new DefaultEphemeralCosmosDatabaseFixture())
{
    protected override async Task DisposeSubjectAsync()
    {
        try
        {
            await base.DisposeSubjectAsync();
        }
        finally
        {
            await CosmosDatabaseFixture.DisposeAsync();
        }
    }
}