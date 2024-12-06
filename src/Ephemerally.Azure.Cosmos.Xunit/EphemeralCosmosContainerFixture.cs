using System.Diagnostics.CodeAnalysis;
using Ephemerally.Xunit;
using Microsoft.Azure.Cosmos;

namespace Ephemerally.Azure.Cosmos.Xunit;

[SuppressMessage("ReSharper", "UseConfigureAwaitFalse")]
public class EphemeralCosmosContainerFixture(ISubjectFixture<Database> cosmosDatabaseFixture)
    : CosmosContainerFixture<EphemeralCosmosContainer>
{
    public EphemeralCosmosContainerFixture() : this(new EphemeralCosmosDatabaseFixture()) { }

    // ReSharper disable once MemberCanBePrivate.Global
    protected ISubjectFixture<Database> CosmosDatabaseFixture { get; } = cosmosDatabaseFixture;

    protected override async Task<EphemeralCosmosContainer> CreateSubjectAsync()
    {
        var database = await CosmosDatabaseFixture.GetOrCreateSubjectAsync();
        return await database.CreateEphemeralContainerAsync();
    }

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