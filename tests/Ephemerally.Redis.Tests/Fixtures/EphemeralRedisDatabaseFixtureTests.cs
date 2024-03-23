using Ephemerally.Redis.Xunit;
using Shouldly;
using StackExchange.Redis;

namespace Ephemerally.Redis.Tests.Fixtures;

// ReSharper disable once InconsistentNaming
public class EphemeralRedisDatabaseFixtureTests_6 : EphemeralRedisDatabaseFixtureTests<EphemeralRedisInstance6>;

// ReSharper disable once InconsistentNaming
public class EphemeralRedisDatabaseFixtureTests_7 : EphemeralRedisDatabaseFixtureTests<EphemeralRedisInstance7>;

[Collection(RedisTestCollection.Name)]
public abstract class EphemeralRedisDatabaseFixtureTests<TRedisFixture> : IAsyncLifetime
    where TRedisFixture : class, IEphemeralRedisFixture, new()
{
    private readonly EphemeralRedisDatabasePoolFixture
        _bigFixture = new BigEphemeralRedisDatabasePoolFixture<TRedisFixture>(),
        _smallFixture = new SmallEphemeralRedisDatabasePoolFixture<TRedisFixture>();

    [RedisFact]
    public async Task Create_database_gets_a_new_database_every_time()
    {
        // Arrange
        // Act
        await using var db1 = _bigFixture.Multiplexer.GetEphemeralDatabase();
        await using var db2 = _bigFixture.Multiplexer.GetEphemeralDatabase();
        await using var db3 = _bigFixture.Multiplexer.GetEphemeralDatabase();
        await using var db4 = _bigFixture.Multiplexer.GetEphemeralDatabase();

        // Assert
        new[]
        {
            db1.Database,
            db2.Database,
            db3.Database,
            db4.Database
        }
        .Distinct()
        .Count()
        .ShouldBe(4);
    }

    [RedisFact]
    public async Task Create_database_should_return_previously_used_database_after_disposal()
    {
        // Arrange
        var db0 = _smallFixture.Multiplexer.GetEphemeralDatabase();
        var db0Id = db0.Database;
        await using var db1 = _smallFixture.Multiplexer.GetEphemeralDatabase();

        // Act
        await db0.DisposeAsync();
        await using var db2 = _smallFixture.Multiplexer.GetEphemeralDatabase();

        // Assert
        db2.Database.ShouldBe(db0Id);
    }

    [RedisFact]
    public async Task Dispose_should_flush_all_included_databases()
    {
        // Arrange
        int[] dbs = [0, 1, 2];
        const string key = nameof(Dispose_should_flush_all_included_databases);
        var values = dbs.Select(_ => Guid.NewGuid().ToString()).ToArray();

        await using var redis = new TRedisFixture().ToDisposable();
        await using var sut = new InTestRedisFixture(redis, dbs);
        await sut.InitializeAsync();
        for (var i = 0; i < dbs.Length; i++)
        {
            var db = sut.Multiplexer.GetEphemeralDatabase(dbs[i]);
            await db.StringSetAsync(key, values[i]);
        }

        // Act
        await sut.DisposeAsync();

        // Assert
        await using var multiplexer = await ConnectionMultiplexer.ConnectAsync(redis.Value.ConnectionString);
        foreach (var db in dbs)
        {
            var actual = await multiplexer.GetDatabase(db).StringGetAsync(key);
            actual.HasValue.ShouldBeFalse();
        }
    }

    [RedisFact]
    public async Task Dispose_should_not_flush_excluded_databases()
    {
        // Arrange
        int[] dbs = [0, 1, 2];
        int[] includedDbs = [1];
        const string key = nameof(Dispose_should_not_flush_excluded_databases);
        var values = dbs.Select(_ => Guid.NewGuid().ToString()).ToArray();

        await using var redis = new TRedisFixture().ToDisposable();
        await using var sut = new InTestRedisFixture(redis, includedDbs);
        await sut.InitializeAsync();

        await using var multiplexer1 = await ConnectionMultiplexer.ConnectAsync(redis.Value.ConnectionString);

        for (var i = 0; i < dbs.Length; i++)
        {
            var db = multiplexer1.GetDatabase(dbs[i]);
            await db.StringSetAsync(key, values[i]);
        }

        await multiplexer1.DisposeAsync();

        foreach (var db in includedDbs)
        {
            sut.Multiplexer.GetDatabase(db).StringGet(key).HasValue.ShouldBeTrue();
        }

        // Act
        await sut.DisposeAsync();

        // Assert
        await using var multiplexer2 = await ConnectionMultiplexer.ConnectAsync(redis.Value.ConnectionString);
        foreach (var db in dbs)
        {
            var actual = await multiplexer2.GetDatabase(db).StringGetAsync(key);
            if (includedDbs.Contains(db))
            {
                actual.HasValue.ShouldBeFalse();
            }
            else
            {
                actual.HasValue.ShouldBeTrue();
            }
        }
    }

    private class InTestRedisFixture(TRedisFixture redis, int[] databases) : EphemeralRedisDatabasePoolFixture(redis)
    {
        protected override Task<EphemeralConnectionMultiplexer> CreateMultiplexerAsync(IConnectionMultiplexer implementation)
        {
            return Task.FromResult(new EphemeralConnectionMultiplexer(implementation, databases));
        }
    }

    #region IAsyncLifetime Members

    public async Task InitializeAsync()
    {
        await _bigFixture.InitializeAsync();
        await _smallFixture.InitializeAsync();
    }

    public async Task DisposeAsync()
    {
        await _bigFixture.DisposeAsync();
        await _smallFixture.DisposeAsync();
    }

    #endregion
}

public class BigEphemeralRedisDatabasePoolFixture<TRedisFixture>()
    : EphemeralRedisDatabasePoolFixture(new TRedisFixture())
    where TRedisFixture : IEphemeralRedisFixture, new()
{
    protected override Task<EphemeralConnectionMultiplexer> CreateMultiplexerAsync(IConnectionMultiplexer implementation)
    {
        return Task.FromResult(new EphemeralConnectionMultiplexer(implementation, [0, 1, 2, 3]));
    }
}

public class SmallEphemeralRedisDatabasePoolFixture<TRedisFixture>()
    : EphemeralRedisDatabasePoolFixture(new TRedisFixture())
    where TRedisFixture : IEphemeralRedisFixture, new()
{
    protected override Task<EphemeralConnectionMultiplexer> CreateMultiplexerAsync(IConnectionMultiplexer implementation)
    {
        return Task.FromResult(new EphemeralConnectionMultiplexer(implementation, [0, 1]));
    }
}

file static class AsyncDisposableExtensions
{
    public static AsyncDisposable<T> ToDisposable<T>(this T asyncLifetime)
        where T : class, IAsyncLifetime => new(asyncLifetime);
}

internal class AsyncDisposable<T>(T value) : IAsyncDisposable
    where T : class, IAsyncLifetime
{
    public T Value { get; } = value;
    public async ValueTask DisposeAsync()
    {
        if (!await Value.TryDisposeAsync())
            await Value.DisposeAsync();
    }

    public static implicit operator T(AsyncDisposable<T> asyncDisposable) => asyncDisposable.Value;
}