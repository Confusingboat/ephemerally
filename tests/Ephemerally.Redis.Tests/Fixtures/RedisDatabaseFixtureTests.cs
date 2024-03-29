using Ephemerally.Redis.Xunit;
using Shouldly;

namespace Ephemerally.Redis.Tests.Fixtures;

[Collection(RedisDatabaseFixtureTestCollection.Name)]
public class RedisDatabaseFixtureTests(RedisDatabaseFixture<RedisDatabaseFixtureTestCollectionFixture> databaseFixture)
    : IClassFixture<RedisDatabaseFixture<RedisDatabaseFixtureTestCollectionFixture>>
{
    [Fact]
    public void Database_should_not_be_null()
    {
        databaseFixture.Database.ShouldNotBeNull();
    }
}

[CollectionDefinition(Name)]
public class RedisDatabaseFixtureTestCollection
    : ICollectionFixture<RedisDatabaseFixtureTestCollectionFixture>
{
    public const string Name = nameof(RedisDatabaseFixtureTestCollection);
}

public class RedisDatabaseFixtureTestCollectionFixture
    : PooledEphemeralRedisMultiplexerFixture<RedisInstanceFixture7>;