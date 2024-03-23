using StackExchange.Redis;

namespace Ephemerally.Redis.xUnit;

public class PooledEphemeralRedisMultiplexerFixture<TEphemeralRedisInstance>()
    : PooledEphemeralRedisMultiplexerFixture(new TEphemeralRedisInstance())
    where TEphemeralRedisInstance : IRedisInstanceFixture, new();

public class PooledEphemeralRedisMultiplexerFixture : EphemeralRedisMultiplexerFixture
{
    public PooledEphemeralRedisMultiplexerFixture() { }
    protected PooledEphemeralRedisMultiplexerFixture(IRedisInstanceFixture redisInstanceFixture)
        : base(redisInstanceFixture) { }

    protected override async Task<IConnectionMultiplexer> CreateMultiplexerAsync()
    {
        var implementation = await base.CreateMultiplexerAsync();
        return await CreatePooledMultiplexerAsync(implementation);
    }
    
    protected virtual Task<PooledConnectionMultiplexer> CreatePooledMultiplexerAsync(IConnectionMultiplexer implementation) =>
        Task.FromResult(new PooledConnectionMultiplexer(implementation));
}