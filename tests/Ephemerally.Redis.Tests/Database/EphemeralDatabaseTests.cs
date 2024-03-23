using Ephemerally.Redis.xUnit;
using StackExchange.Redis;
using Shouldly;

namespace Ephemerally.Redis.Tests.Database;

// ReSharper disable once InconsistentNaming
public class EphemeralDatabaseTests_6(RedisMultiplexerFixture_6 fixture)
    : EphemeralDatabaseTests(fixture),
    IClassFixture<RedisMultiplexerFixture_6>;

// ReSharper disable once InconsistentNaming
public class EphemeralDatabaseTests_7(RedisMultiplexerFixture_7 fixture)
    : EphemeralDatabaseTests(fixture),
    IClassFixture<RedisMultiplexerFixture_7>;

[Collection(RedisTestCollection.Name)]
public abstract class EphemeralDatabaseTests(RedisMultiplexerFixture fixture)
{
    private readonly IConnectionMultiplexer _multiplexer = fixture.Multiplexer;

    [RedisFact]
    public async Task Should_return_a_database_and_flush_it()
    {
        // Arrange
        const string key = nameof(Should_return_a_database_and_flush_it);
        var val = Guid.NewGuid().ToString();

        var sut = _multiplexer.GetEphemeralDatabase();
        await sut.StringSetAsync(key, val);

        // Act
        await sut.DisposeAsync();

        // Assert
        var actual = await sut.StringGetAsync(key);
        actual.HasValue.ShouldBeFalse();
    }

    [RedisFact]
    public async Task User_supplied_database_should_be_flushed()
    {
        // Arrange
        const string key = nameof(User_supplied_database_should_be_flushed);
        var val = Guid.NewGuid().ToString();

        var sut = _multiplexer.GetDatabase().ToEphemeral();
        await sut.StringSetAsync(key, val);

        // Act
        await sut.DisposeAsync();

        // Assert
        var actual = await sut.StringGetAsync(key);
        actual.HasValue.ShouldBeFalse();
    }

    [RedisFact]
    public async Task Should_not_flush_separate_database()
    {
        // Arrange
        const string key = nameof(Should_not_flush_separate_database);
        var val = Guid.NewGuid().ToString();

        var sut = _multiplexer.GetEphemeralDatabase(1);
        await sut.StringSetAsync(key, val);

        var otherDatabase = _multiplexer.GetEphemeralDatabase(2);
        await otherDatabase.StringSetAsync(key, val);

        // Act
        await sut.DisposeAsync();

        // Assert
        var actual = await otherDatabase.StringGetAsync(key);
        actual.HasValue!.ShouldBeTrue();
        actual.ToString().ShouldBeEquivalentTo(val);

        await otherDatabase.DisposeAsync();
    }
}