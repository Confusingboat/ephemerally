using StackExchange.Redis;
using Xunit;

namespace Ephemerally.Redis.Xunit;

public class RedisMultiplexerFixture :
    IAsyncDisposable,
    IAsyncLifetime
{
    private readonly Lazy<Task<IConnectionMultiplexer>> _multiplexer;

    public IConnectionMultiplexer Multiplexer => _multiplexer.Value.Result;

    protected Task<IConnectionMultiplexer> GetMultiplexer() => _multiplexer.Value;

    public RedisMultiplexerFixture()
    {
        _multiplexer = new Lazy<Task<IConnectionMultiplexer>>(CreateMultiplexerAsync);
    }

    protected virtual async Task<IConnectionMultiplexer> CreateMultiplexerAsync() => 
        await ConnectionMultiplexer.ConnectAsync(DefaultLocalRedisInstance.ConnectionString);

    public virtual Task InitializeAsync() => _multiplexer.Value;

    public virtual async Task DisposeAsync()
    {
        if (!_multiplexer.IsValueCreated) return;

        var multiplexer = await GetMultiplexer();
        await multiplexer.DisposeAsync();
    }

    async ValueTask IAsyncDisposable.DisposeAsync() =>
        await ((IAsyncLifetime)this).DisposeAsync();
}

