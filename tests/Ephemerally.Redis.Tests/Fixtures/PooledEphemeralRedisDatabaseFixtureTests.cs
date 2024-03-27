using Ephemerally.Redis.Xunit;
using Shouldly;

namespace Ephemerally.Redis.Tests.Fixtures;

public class PooledEphemeralRedisDatabaseFixtureTests6 : PooledEphemeralRedisDatabaseFixtureTests<RedisInstanceFixture6>;

public class PooledEphemeralRedisDatabaseFixtureTests7 : PooledEphemeralRedisDatabaseFixtureTests<RedisInstanceFixture7>;

[Collection(RedisTestCollection.Name)]
public abstract class PooledEphemeralRedisDatabaseFixtureTests<TRedisFixture> : IAsyncLifetime
    where TRedisFixture : class, IRedisInstanceFixture, new()
{
    // Arrange
    private readonly PooledEphemeralRedisDatabaseFixture<TRedisFixture> _fixture = new();

    [RedisFact]
    public async Task Fixture_database_should_not_be_null()
    {
        // Act
        var database = _fixture.Database;

        // Assert
        database.ShouldNotBeNull();
    }

    #region IAsyncLifetime Members

    public Task InitializeAsync() => _fixture.InitializeAsync();

    public Task DisposeAsync() => _fixture.DisposeAsync();

    #endregion
}