using Ephemerally.Azure.Cosmos.Xunit;

namespace Ephemerally.Azure.Cosmos.Tests;

public class InternalExtensionTests
{
    [Test]
    public async Task GetExpiredContainersAsync_should_return_empty_when_no_containers_present()
    {
        using var client = CosmosEmulator.GetClient();
        await using var db = await client.CreateEphemeralDatabaseAsync();
        var expiredContainers = await db.GetExpiredContainersAsync();
        Assert.That(expiredContainers, Is.Empty);
    }

    [Test]
    public async Task GetExpiredContainersAsync_should_return_empty_when_containers_present_but_none_expired()
    {
        using var client = CosmosEmulator.GetClient();
        await using var db = await client.CreateEphemeralDatabaseAsync();

        // We don't need 'using' here because the containers will be cleaned up by the database
        var container = await db.CreateEphemeralContainerAsync(new EphemeralCreationOptions(TimeSpan.FromMinutes(1)));
        var container2 = await db.CreateEphemeralContainerAsync(new EphemeralCreationOptions(TimeSpan.FromMinutes(1)));

        var expiredContainers = await db.GetExpiredContainersAsync();

        Assert.That(expiredContainers, Is.Empty);
    }

    [Test]
    public async Task GetExpiredContainersAsync_should_return_one_when_containers_present_and_one_expired()
    {
        using var client = CosmosEmulator.GetClient();
        await using var db = await client.CreateEphemeralDatabaseAsync();

        // We don't need 'using' here because the containers will be cleaned up by the database
        var container = await db.CreateEphemeralContainerAsync(new EphemeralCreationOptions(TimeSpan.Zero));
        var container2 = await db.CreateEphemeralContainerAsync(new EphemeralCreationOptions(TimeSpan.FromMinutes(1)));

        Assert.That(await container.ExistsAsync(), Is.True);
        Assert.That(await container2.ExistsAsync(), Is.True);

        var expiredContainers = await db.GetExpiredContainersAsync();

        Assert.That(expiredContainers, Has.One.Items);
    }
}