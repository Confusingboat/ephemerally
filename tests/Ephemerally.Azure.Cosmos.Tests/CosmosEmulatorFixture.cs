namespace Ephemerally.Azure.Cosmos.Tests;

[SetUpFixture]
[FixtureLifeCycle(LifeCycle.SingleInstance)]
public class CosmosEmulatorFixture
{
    [OneTimeSetUp]
    public static async Task OneTimeSetUp()
    {
        await CosmosEmulator.Client.ConnectOrThrowAsync();
    }

    [OneTimeTearDown]
    public static void OneTimeTearDown() => CosmosEmulator.Client.Dispose();
}