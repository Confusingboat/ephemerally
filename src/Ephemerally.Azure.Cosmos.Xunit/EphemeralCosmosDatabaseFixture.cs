using System.Diagnostics.CodeAnalysis;
using Ephemerally.Xunit;
using Microsoft.Azure.Cosmos;

namespace Ephemerally.Azure.Cosmos.Xunit;

[SuppressMessage("ReSharper", "UseConfigureAwaitFalse")]
public class EphemeralCosmosDatabaseFixture(ISubjectFixture<CosmosClient> cosmosClientFixture)
    : CosmosDatabaseFixture<EphemeralCosmosDatabase>
{
    public EphemeralCosmosDatabaseFixture() : this(new CosmosEmulatorClientFixture()) { }

    // ReSharper disable once MemberCanBePrivate.Global
    protected ISubjectFixture<CosmosClient> CosmosClientFixture { get; } = cosmosClientFixture;

    protected override async Task<EphemeralCosmosDatabase> CreateSubjectAsync()
    {
        var client = await CosmosClientFixture.GetOrCreateSubjectAsync();
        return await client.CreateEphemeralDatabaseAsync();
    }
    
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