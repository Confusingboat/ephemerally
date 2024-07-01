using Microsoft.Azure.Cosmos;

namespace Ephemerally.Azure.Cosmos.Xunit;

public static class CosmosEmulator
{
    public static CosmosClient GetClient(Action<CosmosClientOptions> configureOptions = null)
    {
        var options = new CosmosClientOptions
        {
            RequestTimeout = TimeSpan.FromSeconds(30),
            ServerCertificateCustomValidationCallback = (_, _, _) => true,
            ConnectionMode = ConnectionMode.Gateway,
            LimitToEndpoint = true
        };
        configureOptions?.Invoke(options);
        return new(
            AccountEndpoint,
            AuthKey,
            options);
    }

    public const string
        AccountEndpoint = "https://localhost:8081",
        AuthKey = "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==",
        ConnectionString = $"AccountEndpoint={AccountEndpoint};AccountKey={AuthKey};";
}