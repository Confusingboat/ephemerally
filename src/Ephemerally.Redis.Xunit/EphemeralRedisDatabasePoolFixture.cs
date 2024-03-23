using StackExchange.Redis;

namespace Ephemerally.Redis.Xunit;

public class EphemeralRedisDatabasePoolFixture : RedisMultiplexerFixture
{
    public EphemeralRedisDatabasePoolFixture() { }
    protected EphemeralRedisDatabasePoolFixture(IEphemeralRedisFixture redisFixture)
        : base(redisFixture) { }

    protected override async Task<IConnectionMultiplexer> CreateMultiplexerAsync()
    {
        var implementation = await base.CreateMultiplexerAsync();
        return new EphemeralConnectionMultiplexer(implementation);
    }
}