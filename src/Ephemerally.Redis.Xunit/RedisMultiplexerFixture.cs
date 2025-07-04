using Ephemerally.Xunit;
using StackExchange.Redis;
using Xunit;

namespace Ephemerally.Redis.Xunit;

public interface IRedisMultiplexerFixture
{
    IConnectionMultiplexer Multiplexer { get; }
}

public class RedisMultiplexerFixture<TEphemeralRedisInstance>()
    : RedisMultiplexerFixture(new TEphemeralRedisInstance())
    where TEphemeralRedisInstance : IRedisInstanceFixture, new();

public class RedisMultiplexerFixture(ISubjectFixture<IRedisInstance> redisInstanceFixture) 
    : SubjectFixture<IConnectionMultiplexer>, IRedisMultiplexerFixture, IAsyncLifetime, IAsyncDisposable
{
    private bool _disposed;

    protected ISubjectFixture<IRedisInstance> RedisInstanceFixture { get; } = redisInstanceFixture;

    public IConnectionMultiplexer Multiplexer => GetOrCreateSubjectAsync().Result;

    public RedisMultiplexerFixture() : this(UnmanagedDefaultLocalRedisInstanceFixture.DefaultLocalRedisInstanceFixture) { }

    protected override async Task<IConnectionMultiplexer> CreateSubjectAsync() =>
        await ConnectionMultiplexer.ConnectAsync(_redisInstanceFixture.ConnectionString);

    public virtual async Task InitializeAsync()
    {
        await _redisInstanceFixture.InitializeAsync();
    }

    public override async Task DisposeAsync()
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