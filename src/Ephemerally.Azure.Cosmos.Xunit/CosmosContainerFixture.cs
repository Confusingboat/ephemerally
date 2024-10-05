using Ephemerally.Xunit;
using Microsoft.Azure.Cosmos;

namespace Ephemerally.Azure.Cosmos.Xunit;

public abstract class CosmosContainerFixture
    : CosmosContainerFixture<Container>;

public abstract class CosmosContainerFixture<TContainer>
    : CosmosSubjectFixture<TContainer>,
        ISubjectFixture<Container> where TContainer : Container
{
    Task<Container> ISubjectFixture<Container>.GetOrCreateSubjectAsync() =>
        GetOrCreateSubjectAsync().ContinueWith(Container (x) => x.Result);

    public TContainer Container => GetOrCreateSubjectAsync().Result;
}