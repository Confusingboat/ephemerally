using StackExchange.Redis;
using Xunit;

namespace Ephemerally.Redis.Xunit;

public class RedisDatabaseFixture<TMultiplexerFixture>(TMultiplexerFixture multiplexerFixture)
    : RedisDatabaseFixture(multiplexerFixture.Multiplexer)
    where TMultiplexerFixture : IRedisMultiplexerFixture;

public class RedisDatabaseFixture : IAsyncLifetime
{
    private bool _disposed;

    private readonly IConnectionMultiplexer _multiplexer;

    private readonly Lazy<Task<IEphemeralRedisDatabase>> _database;

    public IEphemeralRedisDatabase Database => _database.Value.Result;

    public RedisDatabaseFixture()
    {
        _database = new(CreateDatabaseAsync);
    }

    protected RedisDatabaseFixture(IConnectionMultiplexer multiplexer) : this()
    {
        _multiplexer = multiplexer;
    }

    protected virtual Task<IEphemeralRedisDatabase> CreateDatabaseAsync() =>
        Task.FromResult(_multiplexer.GetEphemeralDatabase());

    public async Task InitializeAsync() => await _database.Value;

    public async Task DisposeAsync()
    {
        if (_disposed || !_database.IsValueCreated) return;

        _disposed = true;

        await Database.DisposeAsync();
    }
}