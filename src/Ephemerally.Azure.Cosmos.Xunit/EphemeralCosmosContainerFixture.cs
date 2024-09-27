using System.Diagnostics.CodeAnalysis;
using Microsoft.Azure.Cosmos;

namespace Ephemerally.Azure.Cosmos.Xunit;

[SuppressMessage("ReSharper", "UseConfigureAwaitFalse")]
public class EphemeralCosmosContainerFixture(ISubjectFixture<Database> cosmosDatabaseFixture)
    : ConsumingSubjectFixture<EphemeralCosmosContainer>
{
    public EphemeralCosmosContainer Container => GetOrCreateSubjectAsync().Result;

    protected override async Task<EphemeralCosmosContainer> CreateSubjectAsync()
    {
        var database = await cosmosDatabaseFixture.GetOrCreateSubjectAsync();
        return await database.CreateEphemeralContainerAsync();
    }

    protected override Task DisposeSubjectAsync() => SafeCosmosDisposeAsync(GetOrCreateSubjectAsync);
}