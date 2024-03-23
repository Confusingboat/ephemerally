using Ephemerally.Redis.xUnit;
using StackExchange.Redis;

namespace Ephemerally;

public static class PublicExtensions
{
    public static ConnectionMultiplexer GetMultiplexer(this IRedisInstanceFixture fixture) =>
        ConnectionMultiplexer.Connect(fixture.ConnectionString);

    public static Task<ConnectionMultiplexer> GetMultiplexerAsync(this IRedisInstanceFixture fixture) =>
        ConnectionMultiplexer.ConnectAsync(fixture.ConnectionString);
}