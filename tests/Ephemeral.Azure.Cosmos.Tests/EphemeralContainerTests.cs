namespace Ephemeral.Azure.Cosmos.Tests;

public class EphemeralContainerTests
{
    [Test]
    public async Task Should_create_container_and_tear_it_down()
    {
        var client = CosmosEmulator.Client;
        await using var databaseAccessor = client.CreateEphemeralDatabaseAccessor();
        var sut = (await databaseAccessor.GetAsync()).CreateEphemeralContainerAccessor();
        var container = await sut.GetAsync();

        Assert.That(await container.ExistsAsync(), Is.True);

        await sut.DisposeAsync();

        Assert.That(await container.ExistsAsync(), Is.False);
    }

    [Test]
    public async Task CleanupBehavior_SelfAndExpired_should_remove_self_and_expired_orphaned_container()
    {
        var client = CosmosEmulator.Client;
        await using var databaseAccessor = client.CreateEphemeralDatabaseAccessor();
        var db = await databaseAccessor.GetAsync();
        var orphanAccessor = db.CreateEphemeralContainerAccessor(new EphemeralOptions(DateTimeOffset.MinValue));
        var orphanContainer = await orphanAccessor.GetAsync();

        Assert.That(await orphanContainer.ExistsAsync(), Is.True);

        await Task.Delay(10);

        var sut = db.CreateEphemeralContainerAccessor(new EphemeralOptions { CleanupBehavior = CleanupBehavior.SelfAndExpired });
        var container = await sut.GetAsync();

        Assert.That(await container.ExistsAsync(), Is.True);

        await sut.DisposeAsync();

        Assert.That(await container.ExistsAsync(), Is.False);
        Assert.That(await orphanContainer.ExistsAsync(), Is.False);
    }

    [Test]
    public async Task CleanupBehavior_SelfOnly_should_remove_self_and_not_remove_unexpired_orphaned_container()
    {
        var client = CosmosEmulator.Client;
        await using var databaseAccessor = client.CreateEphemeralDatabaseAccessor();
        var db = await databaseAccessor.GetAsync();
        var orphanAccessor = db.CreateEphemeralContainerAccessor(new EphemeralOptions(DateTimeOffset.MaxValue));
        var orphanContainer = await orphanAccessor.GetAsync();

        Assert.That(await orphanContainer.ExistsAsync(), Is.True);

        var sut = db.CreateEphemeralContainerAccessor(new EphemeralOptions { CleanupBehavior = CleanupBehavior.SelfOnly });
        var container = await sut.GetAsync();

        Assert.That(await container.ExistsAsync(), Is.True);

        await sut.DisposeAsync();

        Assert.That(await container.ExistsAsync(), Is.False);
        Assert.That(await orphanContainer.ExistsAsync(), Is.True);
    }

    [Test]
    public async Task CleanupBehavior_SelfOnly_should_remove_self_and_not_remove_expired_orphaned_container()
    {
        var client = CosmosEmulator.Client;
        await using var databaseAccessor = client.CreateEphemeralDatabaseAccessor();
        var db = await databaseAccessor.GetAsync();
        var orphanAccessor = db.CreateEphemeralContainerAccessor(new EphemeralOptions(DateTimeOffset.MinValue));
        var orphanContainer = await orphanAccessor.GetAsync();

        Assert.That(await orphanContainer.ExistsAsync(), Is.True);

        var sut = db.CreateEphemeralContainerAccessor(new EphemeralOptions { CleanupBehavior = CleanupBehavior.SelfOnly });
        var container = await sut.GetAsync();

        Assert.That(await container.ExistsAsync(), Is.True);

        await sut.DisposeAsync();

        Assert.That(await container.ExistsAsync(), Is.False);
        Assert.That(await orphanContainer.ExistsAsync(), Is.True);
    }

    [Test]
    public async Task CleanupBehavior_NoCleanup_should_not_remove_anything()
    {
        var client = CosmosEmulator.Client;
        await using var databaseAccessor = client.CreateEphemeralDatabaseAccessor();
        var db = await databaseAccessor.GetAsync();
        var orphanAccessor = db.CreateEphemeralContainerAccessor(new EphemeralOptions(DateTimeOffset.MinValue));
        var orphanContainer = await orphanAccessor.GetAsync();

        Assert.That(await orphanContainer.ExistsAsync(), Is.True);

        var sut = db.CreateEphemeralContainerAccessor(new EphemeralOptions { CleanupBehavior = CleanupBehavior.NoCleanup });
        var container = await sut.GetAsync();

        Assert.That(await container.ExistsAsync(), Is.True);

        await sut.DisposeAsync();

        Assert.That(await container.ExistsAsync(), Is.True);
        Assert.That(await orphanContainer.ExistsAsync(), Is.True);
    }

    [Test]
    public async Task CreationCachingBehavior_Cache_should_always_return_same_Container_instance()
    {
        var client = CosmosEmulator.Client;
        await using var databaseAccessor = client.CreateEphemeralDatabaseAccessor();
        var db = await databaseAccessor.GetAsync();
        var sut = db.CreateEphemeralContainerAccessor(new EphemeralOptions { CreationCachingBehavior = CreationCachingBehavior.Cache });
        var container1 = await sut.GetAsync();
        var container2 = await sut.GetAsync();

        Assert.That(container1, Is.SameAs(container2));
    }

    [Test]
    public async Task CreationCachingBehavior_NoCache_should_always_return_different_Container_instance()
    {
        var client = CosmosEmulator.Client;
        await using var databaseAccessor = client.CreateEphemeralDatabaseAccessor();
        var db = await databaseAccessor.GetAsync();
        var sut = db.CreateEphemeralContainerAccessor(new EphemeralOptions { CreationCachingBehavior = CreationCachingBehavior.NoCache });
        var container1 = await sut.GetAsync();
        var container2 = await sut.GetAsync();

        Assert.That(container1, Is.Not.SameAs(container2));
    }
}