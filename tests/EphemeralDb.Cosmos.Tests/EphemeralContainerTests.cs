using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EphemeralDb.Cosmos.Tests;

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
}