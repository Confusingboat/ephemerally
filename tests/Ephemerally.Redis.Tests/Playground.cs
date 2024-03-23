using Ephemerally.Redis.xUnit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ephemerally.Tests;

namespace Ephemerally.Redis.Tests;

/// <summary>
/// This example uses a default, pre-existing Redis instance on localhost with default port 6379.
/// The fixture itself does not provide any concurrency safety, nor does it perform any automatic cleanup.
/// </summary>
/// <param name="fixture"></param>
public class ExampleUsingFixedDefaultRedisInstance(RedisMultiplexerFixture fixture) : IClassFixture<RedisMultiplexerFixture>
{
    private readonly RedisMultiplexerFixture _fixture = fixture;

    [LocalFact(Skip = "Example only")]
    public async Task ExampleUsage()
    {
        var multiplexer = _fixture
            .Multiplexer                // Start with a basic multiplexer
            .AsEphemeralMultiplexer()   // Enable automatic cleanup of databases for this instance
            .AsPooledMultiplexer();     // Provide concurrency safety for this instance
    }
}