using Azure.Identity;
using Ephemerally.Azure.Cosmos.Xunit;
using Microsoft.Azure.Cosmos;
using Xunit;

namespace Ephemerally.Azure.Cosmos.Tests.Examples;

public class ClientFixtureUsageExampleTests(CosmosEmulatorClientFixture fixture)
    : IClassFixture<CosmosEmulatorClientFixture>
{
    [Fact(Skip = "Example")]
    public void TestUsingClient()
    {
        // Do something with the client
        var db = fixture.Client.GetDatabase("myCoolDatabase");
    }
}

public class CustomCosmosClientFixture : CosmosClientFixture
{
    protected override Task<CosmosClient> CreateSubjectAsync()
    {
        var client = new CosmosClient(
            "https://my-azure-cosmos-db-endpoint:443",
            new DefaultAzureCredential(),
            new CosmosClientOptions
            {
                TokenCredentialBackgroundRefreshInterval = TimeSpan.FromMinutes(3)
            });

        return Task.FromResult(client);
    }
}

public class CustomCosmosClientFixtureUsageExampleTests(CustomCosmosClientFixture fixture)
    : IClassFixture<CustomCosmosClientFixture>
{
    [Fact(Skip = "Example")]
    public void TestUsingClient()
    {
        // Do something with the client
        var db = fixture.Client.GetDatabase("myCustomDatabase");
    }
}