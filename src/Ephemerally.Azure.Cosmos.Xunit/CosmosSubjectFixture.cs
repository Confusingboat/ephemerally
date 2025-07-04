using Ephemerally.Xunit;

namespace Ephemerally.Azure.Cosmos.Xunit;

public abstract class CosmosSubjectFixture<TSubject> : SubjectFixture<TSubject> where TSubject : class
{
    protected override Task DisposeSubjectAsync() => SafeCosmosDisposeAsync(GetOrCreateSubjectAsync);
}