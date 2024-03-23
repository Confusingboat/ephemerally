using StackExchange.Redis;

namespace Ephemerally.Redis.Xunit;

public class EphemeralRedisDatabasePoolFixture : RedisMultiplexerFixture
{
    public EphemeralRedisDatabasePoolFixture() { }
    protected EphemeralRedisDatabasePoolFixture(IEphemeralRedisFixture redisFixture)
        : base(redisFixture) { }

    protected sealed override async Task<IConnectionMultiplexer> CreateMultiplexerAsync()
    {
        var implementation = await base.CreateMultiplexerAsync();
        return await CreateMultiplexerAsync(implementation);
    }

    protected virtual Task<EphemeralConnectionMultiplexer> CreateMultiplexerAsync(IConnectionMultiplexer implementation) =>
        Task.FromResult(new EphemeralConnectionMultiplexer(implementation));
}