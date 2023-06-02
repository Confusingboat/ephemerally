# Ephemerally

## Create and automatically cleanup temporary external resources

Infinitely extensible by design. Just install the packages for the types of resources you'd like to exist ephemerally (or create your own!).

## The Convention
All primary integration points are provided in the main `Ephemerally` namespace, typically as extension methods.

## Example Usage
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
    database.GetContainer("myCoolContainer").ToEphemeral();

    // Resources will be automatically cleaned up
    // as we exit the using scopes

    Assert.Pass();
}
```

## Available Packages

### `Ephemerally`
The core library with the main types. This will be included automatically as a dependency.

### `Ephemerally.Azure`
Placeholder for now to hold general Azure dependencies.

### `Ephemerally.Azure.Cosmos`
Contains types and extension methods for creating and using ephemeral Cosmos DB resources.
