using StackExchange.Redis;

namespace Ephemerally.Redis.Xunit;

public class EphemeralRedisMultiplexerFixture<TEphemeralRedisInstance>()
    : EphemeralRedisMultiplexerFixture(new TEphemeralRedisInstance())
    where TEphemeralRedisInstance : IRedisInstanceFixture, new();

public class EphemeralRedisMultiplexerFixture : RedisMultiplexerFixture
{
    public EphemeralRedisMultiplexerFixture() { }
    protected EphemeralRedisMultiplexerFixture(IRedisInstanceFixture redisInstanceFixture)
        : base(redisInstanceFixture) { }

    public IEphemeralRedisDatabase GetDatabase(int db = -1) => Multiplexer.GetDatabase() as IEphemeralRedisDatabase;

    protected override async Task<IConnectionMultiplexer> CreateMultiplexerAsync()
    {
        var implementation = await base.CreateMultiplexerAsync();
        return await CreateEphemeralMultiplexerAsync(implementation);
    }

    protected virtual Task<EphemeralConnectionMultiplexer> CreateEphemeralMultiplexerAsync(IConnectionMultiplexer implementation) =>
        Task.FromResult(new EphemeralConnectionMultiplexer(implementation));
}