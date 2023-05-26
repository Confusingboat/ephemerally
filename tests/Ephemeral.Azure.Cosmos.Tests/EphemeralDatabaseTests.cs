namespace Ephemeral.Azure.Cosmos.Tests;

public class EphemeralDatabaseTests
{
    [Test]
    public async Task Should_create_database_and_tear_it_down()
    {
        var client = CosmosEmulator.Client;
        var sut = client.CreateEphemeralDatabaseAccessor();
        var db = await sut.GetAsync();

        Assert.That(await db.ExistsAsync(), Is.True);

        await sut.DisposeAsync();

        Assert.That(await db.ExistsAsync(), Is.False);
    }

    [Test]
    public async Task CleanupBehavior_SelfAndExpired_should_remove_self_and_expired_orphaned_database()
    {
        var client = CosmosEmulator.Client;
        var orphanAccessor = client.CreateEphemeralDatabaseAccessor(new EphemeralOptions(TimeSpan.Zero));
        var orphanDb = await orphanAccessor.GetAsync();

        Assert.That(await orphanDb.ExistsAsync(), Is.True);

        var sut = client.CreateEphemeralDatabaseAccessor(new EphemeralOptions { CleanupBehavior = CleanupBehavior.SelfAndExpired });
        var db = await sut.GetAsync();

        Assert.That(await db.ExistsAsync(), Is.True);

        await sut.DisposeAsync();

        Assert.That(await db.ExistsAsync(), Is.False);
        Assert.That(await orphanDb.ExistsAsync(), Is.False);
    }

    [Test]
    public async Task CleanupBehavior_SelfAndExpired_should_remove_self_and_not_remove_unexpired_orphaned_database()
    {
        var client = CosmosEmulator.Client;
        await using var orphanAccessor = client.CreateEphemeralDatabaseAccessor();
        var orphanDb = await orphanAccessor.GetAsync();

        Assert.That(await orphanDb.ExistsAsync(), Is.True);

        var sut = client.CreateEphemeralDatabaseAccessor(new EphemeralOptions { CleanupBehavior = CleanupBehavior.SelfAndExpired });
        var db = await sut.GetAsync();

        Assert.That(await db.ExistsAsync(), Is.True);

        await sut.DisposeAsync();

        Assert.That(await db.ExistsAsync(), Is.False);
        Assert.That(await orphanDb.ExistsAsync(), Is.True);
    }

    [Test]
    public async Task CleanupBehavior_SelfOnly_should_remove_self_only_and_not_remove_expired_orphaned_database()
    {
        var client = CosmosEmulator.Client;
        await using var orphanAccessor = client.CreateEphemeralDatabaseAccessor(new EphemeralOptions(TimeSpan.Zero));
        var orphanDb = await orphanAccessor.GetAsync();

        Assert.That(await orphanDb.ExistsAsync(), Is.True);

        var sut = client.CreateEphemeralDatabaseAccessor(new EphemeralOptions { CleanupBehavior = CleanupBehavior.SelfOnly });
        var db = await sut.GetAsync();

        Assert.That(await db.ExistsAsync(), Is.True);

        await sut.DisposeAsync();

        Assert.That(await db.ExistsAsync(), Is.False);
        Assert.That(await orphanDb.ExistsAsync(), Is.True);
    }

    [Test]
    public async Task CleanupBehavior_NoCleanup_should_not_remove_anything()
    {
        var client = CosmosEmulator.Client;
        await using var orphanAccessor = client.CreateEphemeralDatabaseAccessor(new EphemeralOptions(TimeSpan.Zero));
        var orphanDb = await orphanAccessor.GetAsync();

        Assert.That(await orphanDb.ExistsAsync(), Is.True);

        var sut = client.CreateEphemeralDatabaseAccessor(new EphemeralOptions { CleanupBehavior = CleanupBehavior.NoCleanup });
        var db = await sut.GetAsync();

        Assert.That(await db.ExistsAsync(), Is.True);

        await sut.DisposeAsync();

        Assert.That(await db.ExistsAsync(), Is.True);
        Assert.That(await orphanDb.ExistsAsync(), Is.True);

        await db.DeleteAsync();
    }

    [Test]
    public async Task CreationCachingBehavior_Cache_should_always_return_same_Database_instance()
    {
        var client = CosmosEmulator.Client;
        await using var sut = client.CreateEphemeralDatabaseAccessor(new EphemeralOptions { CreationCachingBehavior = CreationCachingBehavior.Cache });
        var db1 = await sut.GetAsync();
        var db2 = await sut.GetAsync();

        Assert.That(db1, Is.SameAs(db2));
    }

    [Test]
    public async Task CreationCachingBehavior_NoCache_should_always_return_different_Database_instance()
    {
        var client = CosmosEmulator.Client;
        await using var sut = client.CreateEphemeralDatabaseAccessor(new EphemeralOptions { CreationCachingBehavior = CreationCachingBehavior.NoCache });
        var db1 = await sut.GetAsync();
        var db2 = await sut.GetAsync();

        Assert.That(db1, Is.Not.SameAs(db2));
    }
}