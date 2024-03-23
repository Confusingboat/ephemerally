using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ephemerally.Redis.xUnit;
using Shouldly;
using StackExchange.Redis;

namespace Ephemerally.Redis.Tests.Multiplexer;

public class EphemeralConnectionMultiplexerTests6(RedisInstanceFixture6 fixture) : EphemeralConnectionMultiplexerTests<RedisInstanceFixture6>(fixture);

public class EphemeralConnectionMultiplexerTests7(RedisInstanceFixture7 fixture) : EphemeralConnectionMultiplexerTests<RedisInstanceFixture7>(fixture);

public abstract class EphemeralConnectionMultiplexerTests<TRedisInstanceFixture>(TRedisInstanceFixture fixture)
    : IClassFixture<TRedisInstanceFixture>
    where TRedisInstanceFixture : class, IRedisInstanceFixture, new()
{
    private readonly TRedisInstanceFixture _fixture = fixture;

    [Fact]
    public async Task Dispose_should_flush_any_databases_retrieved_by_GetDatabase()
    {
        // Arrange
        int[] dbs = [0, 1, 2];
        int[] includedDbs = [1];
        const string key = nameof(Dispose_should_flush_any_databases_retrieved_by_GetDatabase);
        var value = Guid.NewGuid().ToString();

        await using var rootMultiplexer = await _fixture.GetMultiplexerAsync();
        var multiplexer = rootMultiplexer.AsEphemeralMultiplexer();

        foreach (var db in dbs)
        {
            IConnectionMultiplexer m = includedDbs.Contains(db)
                ? multiplexer
                : rootMultiplexer;

            var database = m.GetDatabase(db);
            await database.StringSetAsync(key, value);
        }

        // Act
        await multiplexer.DisposeAsync();

        // Assert
        await using var newMultiplexer = await _fixture.GetMultiplexerAsync();
        foreach (var db in dbs)
        {
            var actual = await newMultiplexer.GetDatabase(db).StringGetAsync(key);
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
}