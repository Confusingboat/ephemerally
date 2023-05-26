using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ephemeral.Azure.Cosmos.Tests;

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