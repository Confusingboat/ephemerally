using Ephemerally.Azure.Cosmos.Xunit;

namespace Ephemerally.Azure.Cosmos.Tests;

[SetUpFixture]
[FixtureLifeCycle(LifeCycle.SingleInstance)]
public class CosmosEmulatorFixture
{
    [OneTimeSetUp]
    public static async Task OneTimeSetUp()
    {
        using var client = CosmosEmulator.GetClient();
        await client.ConnectOrThrowAsync();
    }
}