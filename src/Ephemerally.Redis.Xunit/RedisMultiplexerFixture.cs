using StackExchange.Redis;
using Xunit;

namespace Ephemerally.Redis.Xunit;

public class RedisMultiplexerFixture<TEphemeralRedisInstance>()
    : RedisMultiplexerFixture(new TEphemeralRedisInstance())
    where TEphemeralRedisInstance : IRedisInstanceFixture, new();

public class RedisMultiplexerFixture : IAsyncLifetime, IAsyncDisposable
{
    private bool _disposed;

    private readonly IRedisInstanceFixture _redisInstanceFixture;
    private readonly Lazy<Task<IConnectionMultiplexer>> _multiplexer;

    public IConnectionMultiplexer Multiplexer => _multiplexer.Value.Result;

    protected Task<IConnectionMultiplexer> GetMultiplexer() => _multiplexer.Value;

    public RedisMultiplexerFixture() : this(UnmanagedDefaultLocalRedisInstanceFixture.DefaultLocalRedisInstanceFixture) { }

    protected RedisMultiplexerFixture(IRedisInstanceFixture redisInstanceFixture)
    {
        _redisInstanceFixture = redisInstanceFixture;
        _multiplexer = new Lazy<Task<IConnectionMultiplexer>>(CreateMultiplexerAsync);
    }

    protected virtual async Task<IConnectionMultiplexer> CreateMultiplexerAsync() =>
        await ConnectionMultiplexer.ConnectAsync(_redisInstanceFixture.ConnectionString);

    public virtual async Task InitializeAsync()
    {
        await _redisInstanceFixture.InitializeAsync();
    }

    public virtual async Task DisposeAsync()
    {
        if (_disposed) return;
        _disposed = true;

        if (!_multiplexer.IsValueCreated) return;

        var multiplexer = await GetMultiplexer();
        await multiplexer.DisposeAsync();
        await _redisInstanceFixture.DisposeAsync();
    }

    async ValueTask IAsyncDisposable.DisposeAsync()
    {
        if (_disposed) return;
        _disposed = true;
        
        await DisposeAsync();
    }
}