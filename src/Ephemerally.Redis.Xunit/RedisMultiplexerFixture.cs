using StackExchange.Redis;
using Xunit;

namespace Ephemerally.Redis.Xunit;

public class RedisMultiplexerFixture<TRedisTestContainerFixture>()
    : RedisMultiplexerFixture(new TRedisTestContainerFixture())
    where TRedisTestContainerFixture : IRedisTestContainerFixture, new();

public class RedisMultiplexerFixture : IAsyncLifetime
{
    private readonly IRedisTestContainerFixture _containerFixture;
    private readonly Lazy<Task<IConnectionMultiplexer>> _multiplexer;

    public IConnectionMultiplexer Multiplexer => _multiplexer.Value.Result;

    protected Task<IConnectionMultiplexer> GetMultiplexer() => _multiplexer.Value;

    public RedisMultiplexerFixture() : this(UnmanagedTestContainerFixture.Instance) { }

    protected RedisMultiplexerFixture(IRedisTestContainerFixture containerFixture)
    {
        _containerFixture = containerFixture;
        _multiplexer = new Lazy<Task<IConnectionMultiplexer>>(CreateMultiplexerAsync);
    }

    protected virtual async Task<IConnectionMultiplexer> CreateMultiplexerAsync() =>
        await ConnectionMultiplexer.ConnectAsync(_containerFixture.ConnectionString);

    public virtual async Task InitializeAsync()
    {
        await _containerFixture.InitializeAsync();
        await _multiplexer.Value;
    }

    public virtual async Task DisposeAsync()
    {
        if (!_multiplexer.IsValueCreated) return;

        var multiplexer = await GetMultiplexer();
        await multiplexer.DisposeAsync();
        await _containerFixture.DisposeAsync();
    }
}