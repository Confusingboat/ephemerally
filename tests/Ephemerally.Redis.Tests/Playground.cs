using Ephemerally.Redis.Xunit;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using Ephemerally.Tests;

namespace Ephemerally.Redis.Tests;

/// <summary>
/// This example uses a default, pre-existing Redis instance on localhost with default port 6379.
/// </summary>
public class ExamplesUsingDefaultUnmanagedRedisInstance(
    RedisMultiplexerFixture basicFixture,
    EphemeralRedisMultiplexerFixture ephemeralFixture,
    PooledEphemeralRedisMultiplexerFixture pooledEphemeralFixture) :
    IClassFixture<RedisMultiplexerFixture>,
    IClassFixture<EphemeralRedisMultiplexerFixture>,
    IClassFixture<PooledEphemeralRedisMultiplexerFixture>
{
    private readonly RedisMultiplexerFixture
        _basicFixture = basicFixture,
        _ephemeralFixture = ephemeralFixture,
        _pooledEphemeralFixture = pooledEphemeralFixture;

    [LocalFact(Skip = "Example only")]
    public async Task BasicExample()
    {
        await using var multiplexer = _basicFixture
            .Multiplexer                // Start with a basic multiplexer
            .AsEphemeralMultiplexer()   // Enable automatic cleanup of databases for this instance
            .AsPooledMultiplexer();     // Provide concurrency safety for this instance
    }

    [LocalFact(Skip = "Example only")]
    public async Task EphemeralExample()
    {
        await using var multiplexer = _ephemeralFixture
            .Multiplexer                // Instance will automatically clean up any databases accessed
            .AsPooledMultiplexer();     // Provide concurrency safety for this instance
    }

    [LocalFact(Skip = "Example only")]
    public async Task PooledEphemeralExample()
    {
        await using var multiplexer = _pooledEphemeralFixture
            .Multiplexer;               // Instance will automatically clean up any databases accessed and provide concurrency safety
    }
}

/// <summary>
/// This example uses a custom Redis instance with testcontainers.
/// By using an abstract base class, we can create a matrix of tests that run on multiple Redis instances.
/// </summary>
/// 
public class CustomRedisInstance : IRedisInstanceFixture
{
    private readonly Lazy<IContainer> _container = new(() =>
        new ContainerBuilder()
            .WithImage("redis:6-alpine")
            .WithPortBinding(6379, true)
            .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(6379))
            .Build());

    protected IContainer Container => _container.Value;

    public ushort PublicPort => Container.GetMappedPublicPort(6379);
    public string ConnectionString => $"localhost:{PublicPort},allowAdmin=true";

    public Task InitializeAsync() => _container.Value.StartAsync();
    public Task DisposeAsync() => _container.TryDisposeAsync().AsTask();
}

public class ExamplesUsingCustomRedisInstance(
    RedisMultiplexerFixture<CustomRedisInstance> basicFixture,
    EphemeralRedisMultiplexerFixture<CustomRedisInstance> ephemeralFixture,
    PooledEphemeralRedisMultiplexerFixture<CustomRedisInstance> pooledEphemeralFixture)
    : ExamplesUsingCustomRedisInstance<CustomRedisInstance>(basicFixture, ephemeralFixture, pooledEphemeralFixture);

public abstract class ExamplesUsingCustomRedisInstance<TRedisInstance>(
    RedisMultiplexerFixture<TRedisInstance> basicFixture,
    EphemeralRedisMultiplexerFixture<TRedisInstance> ephemeralFixture,
    PooledEphemeralRedisMultiplexerFixture<TRedisInstance> pooledEphemeralFixture) :
    IClassFixture<RedisMultiplexerFixture<TRedisInstance>>,
    IClassFixture<EphemeralRedisMultiplexerFixture<TRedisInstance>>,
    IClassFixture<PooledEphemeralRedisMultiplexerFixture<TRedisInstance>>
    where TRedisInstance : IRedisInstanceFixture, new()
{
    private readonly RedisMultiplexerFixture
        _basicFixture = basicFixture,
        _ephemeralFixture = ephemeralFixture,
        _pooledEphemeralFixture = pooledEphemeralFixture;

    [LocalFact(Skip = "Example only")]
    public async Task CustomExample()
    {
        await using var multiplexer = _basicFixture
            .Multiplexer                // Start with a custom multiplexer
            .AsEphemeralMultiplexer()   // Enable automatic cleanup of databases for this instance
            .AsPooledMultiplexer();     // Provide concurrency safety for this instance
    }

    [LocalFact(Skip = "Example only")]
    public async Task EphemeralExample()
    {
        await using var multiplexer = _ephemeralFixture
            .Multiplexer                // Instance will automatically clean up any databases accessed
            .AsPooledMultiplexer();     // Provide concurrency safety for this instance
    }

    [LocalFact(Skip = "Example only")]
    public async Task PooledEphemeralExample()
    {
        await using var multiplexer = _pooledEphemeralFixture
            .Multiplexer;               // Instance will automatically clean up any databases accessed and provide concurrency safety
    }
}