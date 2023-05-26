using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Ephemeral.Azure.Cosmos.Tests;

public class ExtensionTests
{
    [Test]
    public async Task GetExpiredContainersAsync_should_return_empty_when_no_containers_present()
    {
        await using var ephemeral = CosmosEmulator.Client.CreateEphemeralDatabaseAccessor();
        var db = await ephemeral.GetAsync();
        var expiredContainers = await db.GetExpiredContainersAsync();
        Assert.That(expiredContainers, Is.Empty);
    }

    [Test]
    public async Task GetExpiredContainersAsync_should_return_empty_when_containers_present_but_none_expired()
    {
        await using var ephemeral = CosmosEmulator.Client.CreateEphemeralDatabaseAccessor();
        var db = await ephemeral.GetAsync();

        // We don't need 'using' here because the containers will be cleaned up by the database
        var container = db.CreateEphemeralContainerAccessor(new EphemeralOptions(TimeSpan.FromMinutes(1)));
        var container2 = db.CreateEphemeralContainerAccessor(new EphemeralOptions(TimeSpan.FromMinutes(1)));

        await container.GetAsync();
        await container2.GetAsync();

        var expiredContainers = await db.GetExpiredContainersAsync();

        Assert.That(expiredContainers, Is.Empty);
    }

    [Test]
    public async Task GetExpiredContainersAsync_should_return_one_when_containers_present_and_one_expired()
    {
        await using var ephemeral = CosmosEmulator.Client.CreateEphemeralDatabaseAccessor();
        var db = await ephemeral.GetAsync();

        // We don't need 'using' here because the containers will be cleaned up by the database
        var ephemeralContainer = db.CreateEphemeralContainerAccessor(new EphemeralOptions(TimeSpan.Zero));
        var ephemeralContainer2 = db.CreateEphemeralContainerAccessor(new EphemeralOptions(TimeSpan.FromMinutes(1)));

        var container = await ephemeralContainer.GetAsync();
        var container2 = await ephemeralContainer2.GetAsync();

        Assert.That(await container.ExistsAsync(), Is.True);
        Assert.That(await container2.ExistsAsync(), Is.True);

        await Task.Delay(100);

        var expiredContainers = await db.GetExpiredContainersAsync();

        Assert.That(expiredContainers, Has.One.Items);
    }
}