using Microsoft.Azure.Cosmos;

namespace EphemeralDb.Cosmos;

public static class Extensions
{
    public static async IAsyncEnumerable<FeedResponse<T>> ToAsyncEnumerable<T>(this FeedIterator<T> iterator)
    {
        while (iterator.HasMoreResults)
        {
            yield return await iterator.ReadNextAsync();
        }
    }

    public static IAsyncEnumerable<T> SelectResources<T>(this IAsyncEnumerable<FeedResponse<T>> enumerable) =>
        enumerable.SelectMany(x => x.Resource.ToAsyncEnumerable());

}