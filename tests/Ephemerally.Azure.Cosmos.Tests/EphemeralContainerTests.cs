namespace Ephemerally.Azure.Cosmos.Tests;

public class EphemeralContainerTests
{
    [Test]
    public async Task Should_create_container_and_tear_it_down()
    {
        var client = CosmosEmulator.Client;
        await using var db = await client.CreateEphemeralDatabaseAsync();
        var sut = await db.CreateEphemeralContainerAsync();

        Assert.That(await sut.ExistsAsync(), Is.True);

        await sut.DisposeAsync();

        Assert.That(await sut.ExistsAsync(), Is.False);
    }

    [Test]
    public async Task User_supplied_container_should_be_torn_down()
    {
        var client = CosmosEmulator.Client;
        await using var db = await client.CreateEphemeralDatabaseAsync();

        var userSuppliedContainer = (await db.CreateContainerAsync("user-supplied-container", "/id")).Container;

        Assert.That(await userSuppliedContainer.ExistsAsync(), Is.True);

        var sut = userSuppliedContainer.ToEphemeral();

        await sut.DisposeAsync();

        Assert.That(await sut.ExistsAsync(), Is.False);
    }

    [Test]
    public async Task CleanupBehavior_SelfAndExpired_should_remove_self_and_expired_orphaned_container()
    {
        var client = CosmosEmulator.Client;
        await using var db = await client.CreateEphemeralDatabaseAsync();
        
        var orphanContainer = await db.CreateEphemeralContainerAsync(new EphemeralCreationOptions(DateTimeOffset.MinValue));

        Assert.That(await orphanContainer.ExistsAsync(), Is.True);

        var sut = await db.CreateEphemeralContainerAsync(new EphemeralCreationOptions { CleanupBehavior = CleanupBehavior.SelfAndExpired });

        Assert.That(await sut.ExistsAsync(), Is.True);

        await sut.DisposeAsync();

        Assert.That(await sut.ExistsAsync(), Is.False);
        Assert.That(await orphanContainer.ExistsAsync(), Is.False);
    }

    [Test]
    public async Task CleanupBehavior_SelfOnly_should_remove_self_and_not_remove_unexpired_orphaned_container()
    {
        var client = CosmosEmulator.Client;
        await using var db = await client.CreateEphemeralDatabaseAsync();
        var orphanContainer = await db.CreateEphemeralContainerAsync(new EphemeralCreationOptions(DateTimeOffset.MaxValue));

        Assert.That(await orphanContainer.ExistsAsync(), Is.True);

        var sut = await db.CreateEphemeralContainerAsync(new EphemeralCreationOptions { CleanupBehavior = CleanupBehavior.SelfOnly });

        Assert.That(await sut.ExistsAsync(), Is.True);

        await sut.DisposeAsync();

        Assert.That(await sut.ExistsAsync(), Is.False);
        Assert.That(await orphanContainer.ExistsAsync(), Is.True);
    }

    [Test]
    public async Task CleanupBehavior_SelfOnly_should_remove_self_and_not_remove_expired_orphaned_container()
    {
        var client = CosmosEmulator.Client;
        await using var db = await client.CreateEphemeralDatabaseAsync();
        var orphanContainer = await db.CreateEphemeralContainerAsync(new EphemeralCreationOptions(DateTimeOffset.MinValue));

        Assert.That(await orphanContainer.ExistsAsync(), Is.True);

        var sut = await db.CreateEphemeralContainerAsync(new EphemeralCreationOptions { CleanupBehavior = CleanupBehavior.SelfOnly });

        Assert.That(await sut.ExistsAsync(), Is.True);

        await sut.DisposeAsync();

        Assert.That(await sut.ExistsAsync(), Is.False);
        Assert.That(await orphanContainer.ExistsAsync(), Is.True);
    }

    [Test]
    public async Task CleanupBehavior_NoCleanup_should_not_remove_anything()
    {
        var client = CosmosEmulator.Client;
        await using var db = await client.CreateEphemeralDatabaseAsync();
        var orphanContainer = await db.CreateEphemeralContainerAsync(new EphemeralCreationOptions(DateTimeOffset.MinValue));

        Assert.That(await orphanContainer.ExistsAsync(), Is.True);

        var sut = await db.CreateEphemeralContainerAsync(new EphemeralCreationOptions { CleanupBehavior = CleanupBehavior.NoCleanup });

        Assert.That(await sut.ExistsAsync(), Is.True);

        await sut.DisposeAsync();

        Assert.That(await sut.ExistsAsync(), Is.True);
        Assert.That(await orphanContainer.ExistsAsync(), Is.True);
    }
}