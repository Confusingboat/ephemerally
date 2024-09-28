using Microsoft.Azure.Cosmos;
using System.Diagnostics.CodeAnalysis;
using Ephemerally.Xunit;
using Xunit;

namespace Ephemerally.Azure.Cosmos.Xunit;

[SuppressMessage("ReSharper", "UseConfigureAwaitFalse")]
public class CosmosClientFixture : SubjectFixture<CosmosClient>
{
    public CosmosClient Client => GetOrCreateSubjectAsync().Result;

    protected override Task<CosmosClient> CreateSubjectAsync() => Task.FromResult(CosmosEmulator.GetClient());

    protected override Task DisposeSubjectAsync() => SafeCosmosDisposeAsync(GetOrCreateSubjectAsync);
}