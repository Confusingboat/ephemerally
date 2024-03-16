using Ephemerally.Redis.Xunit;
using Shouldly;
using StackExchange.Redis;

namespace Ephemerally.Redis.Tests.Fixtures;

// ReSharper disable once InconsistentNaming
public class EphemeralRedisDatabaseFixtureTests_6(
    BigEphemeralRedisDatabasePoolFixture<RedisTestContainerFixture_6> bigFixture,
    SmallEphemeralRedisDatabasePoolFixture<RedisTestContainerFixture_6> smallFixture)
    : EphemeralRedisDatabaseFixtureTests(bigFixture, smallFixture),
    IClassFixture<BigEphemeralRedisDatabasePoolFixture<RedisTestContainerFixture_6>>,
    IClassFixture<SmallEphemeralRedisDatabasePoolFixture<RedisTestContainerFixture_6>>;

// ReSharper disable once InconsistentNaming
public class EphemeralRedisDatabaseFixtureTests_7(
        BigEphemeralRedisDatabasePoolFixture<RedisTestContainerFixture_7> bigFixture,
        SmallEphemeralRedisDatabasePoolFixture<RedisTestContainerFixture_7> smallFixture)
    : EphemeralRedisDatabaseFixtureTests(bigFixture, smallFixture),
    IClassFixture<BigEphemeralRedisDatabasePoolFixture<RedisTestContainerFixture_7>>,
    IClassFixture<SmallEphemeralRedisDatabasePoolFixture<RedisTestContainerFixture_7>>;

[Collection(RedisTestCollection.Name)]
public abstract class EphemeralRedisDatabaseFixtureTests(
    EphemeralRedisDatabasePoolFixture fixture,
    EphemeralRedisDatabasePoolFixture smallFixture)
{
    private readonly EphemeralRedisDatabasePoolFixture
        _fixture = fixture,
        _smallFixture = smallFixture;


    [Fact]
    public async Task Create_database_gets_a_new_database_every_time()
    {
        // Arrange
        // Act
        await using var db1 = _fixture.Multiplexer.GetEphemeralDatabase();
        await using var db2 = _fixture.Multiplexer.GetEphemeralDatabase();
        await using var db3 = _fixture.Multiplexer.GetEphemeralDatabase();
        await using var db4 = _fixture.Multiplexer.GetEphemeralDatabase();

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

    [Fact]
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
}

public class BigEphemeralRedisDatabasePoolFixture<TRedisTestContainerFixture>()
    : EphemeralRedisDatabasePoolFixture(new TRedisTestContainerFixture())
    where TRedisTestContainerFixture : IRedisTestContainerFixture, new()
{
    protected override async Task<IConnectionMultiplexer> CreateMultiplexerAsync()
    {
        var multiplexer = await base.CreateMultiplexerAsync();
        return new PooledConnectionMultiplexer(multiplexer, [0, 1, 2, 3]);
    }
}

public class SmallEphemeralRedisDatabasePoolFixture<TRedisTestContainerFixture>()
    : EphemeralRedisDatabasePoolFixture(new TRedisTestContainerFixture())
    where TRedisTestContainerFixture : IRedisTestContainerFixture, new()
{
    protected override async Task<IConnectionMultiplexer> CreateMultiplexerAsync()
    {
        var multiplexer = await base.CreateMultiplexerAsync();
        return new PooledConnectionMultiplexer(multiplexer, [0, 1]);
    }
}