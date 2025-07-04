using Ephemerally.Azure.Cosmos.Xunit;
using Xunit;

namespace Ephemerally.Azure.Cosmos.Tests.Examples;

public class DatabaseFixtureUsageExampleTests(EphemeralCosmosDatabaseFixture fixture)
    : IClassFixture<EphemeralCosmosDatabaseFixture>
{
    [Fact(Skip = "Example")]
    public async Task TestUsingDatabase()
    {
        // Ephemeral databases go nicely with ephemeral containers to keep tests isolated
        await using var container = await fixture.Database.CreateEphemeralContainerAsync();
    }
}

public class CustomEphemeralDatabaseFixture() : EphemeralCosmosDatabaseFixture(new CustomCosmosClientFixture());

public class CustomDatabaseFixtureWithCustomClientUsageExampleTests(CustomEphemeralDatabaseFixture fixture)
    : IClassFixture<CustomEphemeralDatabaseFixture>
{
    [Fact(Skip = "Example")]
    public async Task TestUsingDatabase()
    {
        // Ephemeral databases go nicely with ephemeral containers to keep tests isolated
        await using var container = await fixture.Database.CreateEphemeralContainerAsync();
    }
}