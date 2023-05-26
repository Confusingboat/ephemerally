using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ephemeral.Azure.Cosmos.Tests;

[SetUpFixture]
[FixtureLifeCycle(LifeCycle.SingleInstance)]
public class CosmosEmulatorFixture
{
    [OneTimeSetUp]
    public static async Task OneTimeSetUp()
    {
        await CosmosEmulator.Client.ConnectOrThrowAsync();
    }

    [OneTimeTearDown]
    public static void OneTimeTearDown() => CosmosEmulator.Client.Dispose();
}