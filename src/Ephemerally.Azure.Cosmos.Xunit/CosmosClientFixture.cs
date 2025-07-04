using Microsoft.Azure.Cosmos;

namespace Ephemerally.Azure.Cosmos.Xunit;

public abstract class CosmosClientFixture : CosmosSubjectFixture<CosmosClient>
{
    public CosmosClient Client => GetOrCreateSubjectAsync().Result;
}