using Microsoft.Azure.Cosmos;

namespace Ephemerally.Azure.Cosmos.Tests;

internal static class Extensions
{
    public static Task ConnectOrThrowAsync(this CosmosClient client) => client.ReadAccountAsync();

    public static async Task<bool> CanConnectAsync(this CosmosClient client)
    {
        try
        {
            await client.ConnectOrThrowAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }
}