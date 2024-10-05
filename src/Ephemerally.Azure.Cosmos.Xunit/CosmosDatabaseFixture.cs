using Ephemerally.Xunit;
using Microsoft.Azure.Cosmos;

namespace Ephemerally.Azure.Cosmos.Xunit;

public abstract class CosmosDatabaseFixture
    : CosmosDatabaseFixture<Database>;

public abstract class CosmosDatabaseFixture<TDatabase>
    : CosmosSubjectFixture<TDatabase>,
        ISubjectFixture<Database> where TDatabase : Database
{
    Task<Database> ISubjectFixture<Database>.GetOrCreateSubjectAsync() =>
        GetOrCreateSubjectAsync().ContinueWith(Database (x) => x.Result);

    public TDatabase Database => GetOrCreateSubjectAsync().Result;
}