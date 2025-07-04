using Ephemerally.Azure.Cosmos.Xunit;
using Xunit;

namespace Ephemerally.Azure.Cosmos.Tests.Examples;

public class ContainerFixtureUsageExampleTests(EphemeralCosmosContainerFixture fixture)
    : IClassFixture<EphemeralCosmosContainerFixture>
{
    [Fact(Skip = "Example")]
    public async Task TestUsingContainer()
    {
        // Do something with the container
        await fixture.Container.CreateItemAsync(
            new
            {
                id = "12345",
                message = "Hello World!"
            });
    }
}