using Ephemerally.Azure.Cosmos.Xunit;
using Microsoft.Azure.Cosmos;

namespace Ephemerally.Azure.Cosmos.Tests;

public class EphemeralContainerTests
{
    [Test]
    public async Task Should_create_container_and_tear_it_down()
    {
        using var client = CosmosEmulator.GetClient();
        await using var db = await client.CreateEphemeralDatabaseAsync();
        var sut = await db.CreateEphemeralContainerAsync();

        Assert.That(await sut.ExistsAsync(), Is.True);

        await sut.DisposeAsync();

        Assert.That(await sut.ExistsAsync(), Is.False);
    }

    [Test]
    public async Task User_supplied_container_should_be_torn_down()
    {
        using var client = CosmosEmulator.GetClient();
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
        using var client = CosmosEmulator.GetClient();
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
        using var client = CosmosEmulator.GetClient();
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
        using var client = CosmosEmulator.GetClient();
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
        using var client = CosmosEmulator.GetClient();
        await using var db = await client.CreateEphemeralDatabaseAsync();
        var orphanContainer = await db.CreateEphemeralContainerAsync(new EphemeralCreationOptions(DateTimeOffset.MinValue));

        Assert.That(await orphanContainer.ExistsAsync(), Is.True);

        var sut = await db.CreateEphemeralContainerAsync(new EphemeralCreationOptions { CleanupBehavior = CleanupBehavior.NoCleanup });

        Assert.That(await sut.ExistsAsync(), Is.True);

        await sut.DisposeAsync();

        Assert.That(await sut.ExistsAsync(), Is.True);
        Assert.That(await orphanContainer.ExistsAsync(), Is.True);
    }

    [Test]
    public async Task Should_create_container_when_container_properties_Id_is_provided()
    {
        const string userSuppliedId = "user-supplied-id";

        using var client = CosmosEmulator.GetClient();
        await using var db = await client.CreateEphemeralDatabaseAsync();

        await using var sut = await db.CreateEphemeralContainerAsync(containerProperties: new ContainerProperties { Id = userSuppliedId });

        Assert.That(await sut.ExistsAsync(), Is.True);
        Assert.That(sut.Id, Is.EqualTo(userSuppliedId));
    }

    [Test]
    public async Task Should_create_container_when_container_properties_Id_is_not_provided()
    {
        using var client = CosmosEmulator.GetClient();
        await using var db = await client.CreateEphemeralDatabaseAsync();

        await using var sut = await db.CreateEphemeralContainerAsync(containerProperties: new());

        Assert.That(await sut.ExistsAsync(), Is.True);
    }

    [Test]
    public async Task Should_create_container_when_container_properties_PartitionKeyPath_is_provided()
    {
        const string userSuppliedKey = "/userSuppliedKey";

        using var client = CosmosEmulator.GetClient();
        await using var db = await client.CreateEphemeralDatabaseAsync();

        await using var sut = await db.CreateEphemeralContainerAsync(containerProperties: new ContainerProperties { PartitionKeyPath = userSuppliedKey });

        Assert.That(await sut.ExistsAsync(), Is.True);
    }

    [Test]
    public async Task Should_create_container_when_container_properties_PartitionKeyPath_is_not_provided()
    {
        using var client = CosmosEmulator.GetClient();
        await using var db = await client.CreateEphemeralDatabaseAsync();

        await using var sut = await db.CreateEphemeralContainerAsync(containerProperties: new());

        Assert.That(await sut.ExistsAsync(), Is.True);
    }
}