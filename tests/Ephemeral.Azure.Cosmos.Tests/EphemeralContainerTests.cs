namespace Ephemeral.Azure.Cosmos.Tests;

public class EphemeralContainerTests
{
    [Test]
    public async Task Should_create_container_and_tear_it_down()
    {
        var client = CosmosEmulator.Client;
        await using var databaseAccessor = client.CreateEphemeralDatabaseAsync();
        var sut = (await databaseAccessor.GetAsync()).CreateEphemeralContainerAsync();
        var container = await sut.GetAsync();

        Assert.That(await container.ExistsAsync(), Is.True);

        await sut.DisposeAsync();

        Assert.That(await container.ExistsAsync(), Is.False);
    }

    [Test]
    public async Task CleanupBehavior_SelfAndExpired_should_remove_self_and_expired_orphan_container()
    {
        var client = CosmosEmulator.Client;
        await using var databaseAccessor = client.CreateEphemeralDatabaseAsync();
        var db = await databaseAccessor.GetAsync();
        var orphanAccessor = db.CreateEphemeralContainerAsync(new EphemeralOptions { Expiration = DateTimeOffset.MinValue });
        var orphanContainer = await orphanAccessor.GetAsync();

        Assert.That(await orphanContainer.ExistsAsync(), Is.True);

        await Task.Delay(10);

        var sut = db.CreateEphemeralContainerAsync(new EphemeralOptions { CleanupBehavior = CleanupBehavior.SelfAndExpired });
        var container = await sut.GetAsync();

        Assert.That(await container.ExistsAsync(), Is.True);

        await sut.DisposeAsync();

        Assert.That(await container.ExistsAsync(), Is.False);
        Assert.That(await orphanContainer.ExistsAsync(), Is.False);
    }
}