using System.Diagnostics.CodeAnalysis;
using Ephemerally.Xunit;
using Microsoft.Azure.Cosmos;

namespace Ephemerally.Azure.Cosmos.Xunit;

[SuppressMessage("ReSharper", "UseConfigureAwaitFalse")]
public class EphemeralCosmosContainerFixture(ISubjectFixture<Database> cosmosDatabaseFixture)
    : CosmosContainerFixture<EphemeralCosmosContainer>
{
    protected override async Task<EphemeralCosmosContainer> CreateSubjectAsync()
    {
        var database = await cosmosDatabaseFixture.GetOrCreateSubjectAsync();
        return await database.CreateEphemeralContainerAsync();
    }
}