using Ephemerally.Redis.Xunit;
using StackExchange.Redis;

namespace Ephemerally;

public static class PublicExtensions
{
    extension(IRedisInstanceFixture fixture)
    {
        public ConnectionMultiplexer GetMultiplexer() =>
            ConnectionMultiplexer.Connect(fixture.ConnectionString);

        public Task<ConnectionMultiplexer> GetMultiplexerAsync() =>
            ConnectionMultiplexer.ConnectAsync(fixture.ConnectionString);
    }
}