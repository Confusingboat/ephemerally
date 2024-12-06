using Ephemerally.Azure.Cosmos.Xunit;

namespace Ephemerally.Azure.Cosmos.Tests;

public class EphemeralDatabaseTests
{
    [Test]
    public async Task Should_create_database_and_tear_it_down()
    {
        using var client = CosmosEmulator.GetClient();
        var sut = await client.CreateEphemeralDatabaseAsync();

        Assert.That(await sut.ExistsAsync(), Is.True);

        await sut.DisposeAsync();

        Assert.That(await sut.ExistsAsync(), Is.False);
    }

    [Test]
    public async Task User_supplied_database_should_be_torn_down()
    {
        using var client = CosmosEmulator.GetClient();

        var userSuppliedDatabase = (await client.CreateDatabaseAsync("user-supplied-database")).Database;

        Assert.That(await userSuppliedDatabase.ExistsAsync(), Is.True);

        var sut = userSuppliedDatabase.ToEphemeral();

        await sut.DisposeAsync();

        Assert.That(await sut.ExistsAsync(), Is.False);
    }

    [Test]
    public async Task CleanupBehavior_SelfAndExpired_should_remove_self_and_expired_orphaned_database()
    {
        using var client = CosmosEmulator.GetClient();
        var orphanDb = await client.CreateEphemeralDatabaseAsync(new EphemeralCreationOptions(TimeSpan.Zero));

        Assert.That(await orphanDb.ExistsAsync(), Is.True);

        var sut = await client.CreateEphemeralDatabaseAsync(new EphemeralCreationOptions { CleanupBehavior = CleanupBehavior.SelfAndExpired });

        Assert.That(await sut.ExistsAsync(), Is.True);

        await sut.DisposeAsync();

        Assert.That(await sut.ExistsAsync(), Is.False);
        Assert.That(await orphanDb.ExistsAsync(), Is.False);
    }

    [Test]
    public async Task CleanupBehavior_SelfAndExpired_should_remove_self_and_not_remove_unexpired_orphaned_database()
    {
        using var client = CosmosEmulator.GetClient();
        await using var orphanDb = await client.CreateEphemeralDatabaseAsync();

        Assert.That(await orphanDb.ExistsAsync(), Is.True);

        var sut = await client.CreateEphemeralDatabaseAsync(new EphemeralCreationOptions { CleanupBehavior = CleanupBehavior.SelfAndExpired });

        Assert.That(await sut.ExistsAsync(), Is.True);

        await sut.DisposeAsync();

        Assert.That(await sut.ExistsAsync(), Is.False);
        Assert.That(await orphanDb.ExistsAsync(), Is.True);
    }

    [Test]
    public async Task CleanupBehavior_SelfOnly_should_remove_self_only_and_not_remove_expired_orphaned_database()
    {
        using var client = CosmosEmulator.GetClient();
        await using var orphanDb = await client.CreateEphemeralDatabaseAsync(new EphemeralCreationOptions(TimeSpan.Zero));

        Assert.That(await orphanDb.ExistsAsync(), Is.True);

        var sut = await client.CreateEphemeralDatabaseAsync(new EphemeralCreationOptions { CleanupBehavior = CleanupBehavior.SelfOnly });

        Assert.That(await sut.ExistsAsync(), Is.True);

        await sut.DisposeAsync();

        Assert.That(await sut.ExistsAsync(), Is.False);
        Assert.That(await orphanDb.ExistsAsync(), Is.True);
    }

    [Test]
    public async Task CleanupBehavior_NoCleanup_should_not_remove_anything()
    {
        using var client = CosmosEmulator.GetClient();
        await using var orphanDb = await client.CreateEphemeralDatabaseAsync(new EphemeralCreationOptions(TimeSpan.Zero));

        Assert.That(await orphanDb.ExistsAsync(), Is.True);

        var sut = await client.CreateEphemeralDatabaseAsync(new EphemeralCreationOptions { CleanupBehavior = CleanupBehavior.NoCleanup });

        Assert.That(await sut.ExistsAsync(), Is.True);

        await sut.DisposeAsync();

        Assert.That(await sut.ExistsAsync(), Is.True);
        Assert.That(await orphanDb.ExistsAsync(), Is.True);

        await sut.DeleteAsync();
    }
}