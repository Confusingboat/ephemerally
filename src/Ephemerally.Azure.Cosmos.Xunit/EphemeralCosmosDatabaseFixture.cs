using System.Diagnostics.CodeAnalysis;
using Ephemerally.Xunit;
using Microsoft.Azure.Cosmos;

namespace Ephemerally.Azure.Cosmos.Xunit;

[SuppressMessage("ReSharper", "UseConfigureAwaitFalse")]
public class EphemeralCosmosDatabaseFixture(ISubjectFixture<CosmosClient> cosmosClientFixture)
    : CosmosDatabaseFixture<EphemeralCosmosDatabase>
{
    protected override async Task<EphemeralCosmosDatabase> CreateSubjectAsync()
    {
        var client = await cosmosClientFixture.GetOrCreateSubjectAsync();
        return await client.CreateEphemeralDatabaseAsync();
    }
}