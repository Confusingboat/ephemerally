using StackExchange.Redis;

namespace Ephemerally.Redis.xUnit;

public class EphemeralRedisMultiplexerFixture<TEphemeralRedisInstance>()
    : EphemeralRedisMultiplexerFixture(new TEphemeralRedisInstance())
    where TEphemeralRedisInstance : IRedisInstanceFixture, new();

public class EphemeralRedisMultiplexerFixture : RedisMultiplexerFixture
{
    public EphemeralRedisMultiplexerFixture() { }
    protected EphemeralRedisMultiplexerFixture(IRedisInstanceFixture redisInstanceFixture)
        : base(redisInstanceFixture) { }

    protected override async Task<IConnectionMultiplexer> CreateMultiplexerAsync()
    {
        var implementation = await base.CreateMultiplexerAsync();
        return await CreateEphemeralMultiplexerAsync(implementation);
    }

    protected virtual Task<EphemeralConnectionMultiplexer> CreateEphemeralMultiplexerAsync(IConnectionMultiplexer implementation) =>
        Task.FromResult(new EphemeralConnectionMultiplexer(implementation));
}