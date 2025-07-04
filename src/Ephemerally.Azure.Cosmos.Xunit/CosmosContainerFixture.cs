using Ephemerally.Xunit;
using Microsoft.Azure.Cosmos;

namespace Ephemerally.Azure.Cosmos.Xunit;

public interface ICosmosContainerFixture<out TContainer>
    : ISubjectFixture<Container>
    where TContainer : Container
{
    TContainer Container { get; }
}

public abstract class CosmosContainerFixture
    : CosmosContainerFixture<Container>;

public abstract class CosmosContainerFixture<TContainer>
    : CosmosSubjectFixture<TContainer>, ICosmosContainerFixture<TContainer>
    where TContainer : Container
{
    Task<Container> ISubjectFixture<Container>.GetOrCreateSubjectAsync() =>
        GetOrCreateSubjectAsync().ContinueWith(Container (x) => x.Result);

    public TContainer Container => GetOrCreateSubjectAsync().Result;
}