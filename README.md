# Ephemerally 

![NuGet Downloads](https://img.shields.io/nuget/dt/Ephemerally)
![NuGet Version](https://img.shields.io/nuget/vpre/Ephemerally)

### Packages available on [nuget.org](https://www.nuget.org/packages?q=Ephemerally)

### Full documentation available in [the wiki](../../wiki)

## Create and automatically cleanup temporary external resources

#### Extensible by design. Just install the appropriate package and start using resources ephemerally.

## Why use this instead of `testcontainers`?

Creating and tearing down entire containers per-test can be time-intensive and slow things down considerably, and not providing proper isolation can have other unintended side effects like race conditions or contamination of your test results.

The best use cases will combine both approaches, using `testcontainers` to provide the infrastructure for the ephemeral resources created by `Ephemerally`, for example using `testcontainers` to provide a Redis instance for the `PooledEphemeralRedisMultiplexerFixture`.

There are also use cases where it is not possible to use temporary containers with something like `testcontainers`, for example when:
- there is no containerized version of a resource
- policy forbids it
- technical challenges make it impractical
- there is a specific case like performance testing, where you want or need an actual, full-scale resource

## Can I only use this in tests?

While the primary use case is intended to be for testing, the projects are organized to allow the core functionality to be used anywhere without dependence on a test framework.
This is why the test-specific code is organized into separate projects like `Ephemerally.Azure.Cosmos.Xunit`. For example, if you would like to use ephemeral databases or containers for Cosmos in production code, you would only add `Ephemerally.Azure.Cosmos`.

## Integration
Basic integration points are provided in the main `Ephemerally` namespace, typically as extension methods.

Test fixtures will be under their respective package names (e.g. `Ephemerally.Azure.Cosmos.Xunit`).

## Create Resources Ephemerally
To create a temporary Cosmos database and container for your persistence unit tests:
1. Install the relevant package:  
```
dotnet add package Ephemerally.Azure.Cosmos
```
2. Import the namespace
```csharp
using Ephemerally;
```
3. Create and use your resources
```csharp
[Test]
public async Task Test_should_pass()
{
    // Create the client to connect to the emulator (or any other instance)
    var client = new CosmosClient(
        "https://localhost:8081",
        "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==");

    // Create a database
    await using var database = await client.CreateEphemeralDatabaseAsync();

    // Now let's create a container
    await using var container = await database.CreateEphemeralContainerAsync();

    // You can even bring your own database or container to clean up automatically
    await using var myCoolContainer = database.GetContainer("myCoolContainer").ToEphemeral();

    // Resources will be automatically cleaned up
    // as we exit the using scopes

    Assert.Pass();
}
```

## Test Fixtures
To utilize the premade fixtures (currently only available for xUnit):
1. Install the appropriate package:
```
dotnet add package Ephemerally.Azure.Cosmos.Xunit
```
2. Import the namespace
```csharp
using Ephemerally.Azure.Cosmos.Xunit;
```
3. Add a fixture to your test class
```csharp
using Ephemerally.Azure.Cosmos.Xunit;
using Xunit;

public class DatabaseFixtureUsageExampleTests(EphemeralCosmosDatabaseFixture fixture)
    : IClassFixture<EphemeralCosmosDatabaseFixture>
{
    // TODO: Tests go here
}
```
4. Use the fixture
```csharp
using Ephemerally;
using Ephemerally.Azure.Cosmos.Xunit;
using Xunit;

public class DatabaseFixtureUsageExampleTests(EphemeralCosmosDatabaseFixture fixture)
    : IClassFixture<EphemeralCosmosDatabaseFixture>
{
    [Fact]
    public async Task TestUsingDatabase()
    {
        // Ephemeral databases go well with ephemeral containers to keep tests isolated
        await using var container = await fixture.Database.CreateEphemeralContainerAsync();
    }
}
```

## Available Packages

### `Ephemerally`
Base package with shared types. This will be included automatically as a transitive dependency.

### `Ephemerally.Azure`
Placeholder for now to hold general Azure dependencies.

### `Ephemerally.Azure.Cosmos`
Contains types and extension methods for creating and using ephemeral Cosmos DB resources.

### `Ephemerally.Azure.Cosmos.Xunit`
xUnit fixtures for simplifying test integration with `Ephemerally.Azure.Cosmos`

### `Ephemerally.Redis`
Classes and methods for creating and managing the lifecycle of ephemeral Redis resources

### `Ephemerally.Redis.Xunit`
xUnit fixtures for simplifying test integration with `Ephemerally.Redis`