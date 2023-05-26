namespace Ephemeral.Azure.Cosmos.Tests;

public class EphemeralContainerTests
{
    [Test]
    public async Task Should_create_container_and_tear_it_down()
    {
        var client = CosmosEmulator.Client;
        await using var db = client.CreateEphemeralDatabaseAsync();
        var sut = (await db.GetAsync()).CreateEphemeralContainerAsync();
        var container = await sut.GetAsync();

        Assert.That(await container.ExistsAsync(), Is.True);

        await sut.DisposeAsync();

        Assert.That(await container.ExistsAsync(), Is.False);
    }

    [Test]
    public async Task CleanupBehavior_SelfAndExpired_should_remove_self_and_expired_orphan_container()
    {
        var client = CosmosEmulator.Client;
        await using var db = client.CreateEphemeralDatabaseAsync(new EphemeralOptions { ContainerLifetime = TimeSpan.Zero });
        var orphan = (await db.GetAsync()).CreateEphemeralContainerAsync();
        var orphanContainer = await orphan.GetAsync();

        Assert.That(await orphanContainer.ExistsAsync(), Is.True);

        

        var sut = (await db.GetAsync()).CreateEphemeralContainerAsync(new EphemeralOptions { CleanupBehavior = CleanupBehavior.SelfAndExpired });
        var container = await sut.GetAsync();

        Assert.That(await container.ExistsAsync(), Is.True);

        await Task.Delay(2000);

        await sut.DisposeAsync();

        Assert.That(await container.ExistsAsync(), Is.False);
        Assert.That(await orphanContainer.ExistsAsync(), Is.False);
    }
}