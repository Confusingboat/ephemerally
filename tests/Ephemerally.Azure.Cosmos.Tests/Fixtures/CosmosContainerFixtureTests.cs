using Ephemerally.Azure.Cosmos.Xunit;
using Microsoft.Azure.Cosmos;
using Shouldly;

namespace Ephemerally.Azure.Cosmos.Tests.Fixtures;

public class DefaultEphemeralCosmosContainerFixtureTests
    : CosmosContainerFixtureTests<DefaultEphemeralCosmosContainerFixture>;

public abstract class CosmosContainerFixtureTests<TFixture>
    where TFixture : ICosmosContainerFixture<Container>, new()
{
    [Test]
    public async Task Fixture_provides_usable_cosmos_container()
    {
        // Arrange
        await using var fixture = new TFixture();
        await fixture.InitializeAsync();

        // Act
        var exists = await fixture.Container.ExistsAsync();

        // Assert
        exists.ShouldBeTrue();
    }
}