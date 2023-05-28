# Ephemeral

## Create and automatically cleanup temporary external resources

Infinitely extensible by design. Just install the packages for the types of resources you'd like to exist ephemerally.

## The Convention
All primary integration points are provided in the main `Ephemeral` namespace, typically as extension methods.

## Example Usage
To create a temporary Cosmos database and container for your persistence unit tests:
1. Install the relevant package:  
```
dotnet add package Ephemeral.Azure.Cosmos
```
2. Import the namespace
```csharp
using Ephemeral;
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

    // Create the accessor, which is what manages the resource lifecycle
    await using var databaseAccessor = client.CreateEphemeralDatabaseAccessor();

    // Get an instance of the new database
    // (creation actually happens here)
    var db = await databaseAccessor.GetAsync();

    // Now let's create the container
    await using var containerAccessor = db.CreateEphemeralContainerAccessor();

    // Get the instance of the container
    var container = await containerAccessor.GetAsync();

    // Do whatever you want here

    // Resources will be automatically cleaned up
    // as we exit the using scopes
}
```

## Available Packages

### `Ephemeral`
The core library with the main types. This will be included automatically as a dependency