using Ephemerally.Azure.Cosmos.Xunit;
using Shouldly;

namespace Ephemerally.Azure.Cosmos.Tests.Fixtures;

public class DefaultCosmosEmulatorClientFixtureTests : CosmosClientFixtureTests<DefaultCosmosEmulatorClientFixture>;

public abstract class CosmosClientFixtureTests<TFixture> where TFixture : CosmosClientFixture, new()
{
    [Test]
    public async Task Fixture_provides_usable_cosmos_client()
    {
        // Arrange
        await using var fixture = new TFixture();
        await fixture.InitializeAsync();

        // Act
        var canConnect = await fixture.Client.CanConnectAsync();

        // Assert
        canConnect.ShouldBeTrue();
    }
}