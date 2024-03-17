using StackExchange.Redis;

namespace Ephemerally.Redis.Xunit;

public class EphemeralRedisDatabasePoolFixture : RedisMultiplexerFixture
{
    public EphemeralRedisDatabasePoolFixture() { }
    protected EphemeralRedisDatabasePoolFixture(IRedisTestContainerFixture containerFixture)
        : base(containerFixture) { }

    protected override async Task<IConnectionMultiplexer> CreateMultiplexerAsync()
    {
        var implementation = await base.CreateMultiplexerAsync();
        return new EphemeralConnectionMultiplexer(implementation);
    }
}