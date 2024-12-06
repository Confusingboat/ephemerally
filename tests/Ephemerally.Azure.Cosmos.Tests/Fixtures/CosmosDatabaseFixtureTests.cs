using Ephemerally.Azure.Cosmos.Xunit;
using Microsoft.Azure.Cosmos;
using Shouldly;

namespace Ephemerally.Azure.Cosmos.Tests.Fixtures;

public class DefaultEphemeralCosmosDatabaseFixtureTests
    : CosmosDatabaseFixtureTests<DefaultEphemeralCosmosDatabaseFixture>;

public abstract class CosmosDatabaseFixtureTests<TFixture>
    where TFixture : ICosmosDatabaseFixture<Database>, new()
{
    [Test]
    public async Task Fixture_provides_usable_cosmos_database()
    {
        // Arrange
        await using var fixture = new TFixture();
        await fixture.InitializeAsync();

        // Act
        var exists = await fixture.Database.ExistsAsync();

        // Assert
        exists.ShouldBeTrue();
    }
}