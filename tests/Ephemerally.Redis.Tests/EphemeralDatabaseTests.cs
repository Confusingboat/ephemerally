using Ephemerally.Redis.Xunit;
using StackExchange.Redis;

namespace Ephemerally.Redis.Tests;

public class EphemeralDatabaseTests(RedisMultiplexerFixture fixture) : IClassFixture<RedisMultiplexerFixture>
{
    private readonly IConnectionMultiplexer _multiplexer = fixture.Multiplexer;

    [Fact]
    public void Should_return_a_database_and_flush_it()
    {

    }
}