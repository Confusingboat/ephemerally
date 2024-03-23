using Ephemerally.Redis.Xunit;
using Shouldly;
using StackExchange.Redis;

namespace Ephemerally.Redis.Tests.Fixtures;

// ReSharper disable once InconsistentNaming
public class EphemeralRedisDatabaseFixtureTests_6(
    BigEphemeralRedisDatabasePoolFixture<EphemeralRedisInstance6> bigFixture,
    SmallEphemeralRedisDatabasePoolFixture<EphemeralRedisInstance6> smallFixture)
    : EphemeralRedisDatabaseFixtureTests(bigFixture, smallFixture),
    IClassFixture<BigEphemeralRedisDatabasePoolFixture<EphemeralRedisInstance6>>,
    IClassFixture<SmallEphemeralRedisDatabasePoolFixture<EphemeralRedisInstance6>>;

// ReSharper disable once InconsistentNaming
public class EphemeralRedisDatabaseFixtureTests_7(
        BigEphemeralRedisDatabasePoolFixture<EphemeralRedisInstance7> bigFixture,
        SmallEphemeralRedisDatabasePoolFixture<EphemeralRedisInstance7> smallFixture)
    : EphemeralRedisDatabaseFixtureTests(bigFixture, smallFixture),
    IClassFixture<BigEphemeralRedisDatabasePoolFixture<EphemeralRedisInstance7>>,
    IClassFixture<SmallEphemeralRedisDatabasePoolFixture<EphemeralRedisInstance7>>;

[Collection(RedisTestCollection.Name)]
public abstract class EphemeralRedisDatabaseFixtureTests(
    EphemeralRedisDatabasePoolFixture bigFixture,
    EphemeralRedisDatabasePoolFixture smallFixture)
{
    private readonly EphemeralRedisDatabasePoolFixture
        _bigFixture = bigFixture,
        _smallFixture = smallFixture;

    [Fact]
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
    where TRedisTestContainerFixture : IEphemeralRedisFixture, new()
{
    protected override async Task<IConnectionMultiplexer> CreateMultiplexerAsync()
    {
        var multiplexer = await base.CreateMultiplexerAsync();
        return new EphemeralConnectionMultiplexer(multiplexer, [0, 1, 2, 3]);
    }
}

public class SmallEphemeralRedisDatabasePoolFixture<TRedisTestContainerFixture>()
    : EphemeralRedisDatabasePoolFixture(new TRedisTestContainerFixture())
    where TRedisTestContainerFixture : IEphemeralRedisFixture, new()
{
    protected override async Task<IConnectionMultiplexer> CreateMultiplexerAsync()
    {
        var multiplexer = await base.CreateMultiplexerAsync();
        return new EphemeralConnectionMultiplexer(multiplexer, [0, 1]);
    }
}