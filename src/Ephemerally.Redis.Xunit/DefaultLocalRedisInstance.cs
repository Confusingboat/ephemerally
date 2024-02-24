using StackExchange.Redis;

namespace Ephemerally.Redis.Xunit;

public class DefaultLocalRedisInstance
{
    public static ConnectionMultiplexer Multiplexer { get; } = ConnectionMultiplexer.Connect(ConnectionString);

    public const string ConnectionString = "localhost:6379";
}