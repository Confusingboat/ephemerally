using System.Diagnostics.CodeAnalysis;
using Microsoft.Azure.Cosmos;

namespace Ephemerally.Azure.Cosmos.Xunit;

[SuppressMessage("ReSharper", "UseConfigureAwaitFalse")]
public class EphemeralCosmosDatabaseFixture(ISubjectFixture<CosmosClient> cosmosClientFixture)
    : ConsumingSubjectFixture<EphemeralCosmosDatabase>
{
    public EphemeralCosmosDatabase Database => GetOrCreateSubjectAsync().Result;

    protected override async Task<EphemeralCosmosDatabase> CreateSubjectAsync()
    {
        var client = await cosmosClientFixture.GetOrCreateSubjectAsync();
        return await client.CreateEphemeralDatabaseAsync();
    }

    protected override Task DisposeSubjectAsync() => SafeCosmosDisposeAsync(GetOrCreateSubjectAsync);
}