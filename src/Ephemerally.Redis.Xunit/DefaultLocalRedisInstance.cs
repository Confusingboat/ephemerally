using StackExchange.Redis;

namespace Ephemerally.Redis.Xunit;

public class DefaultLocalRedisInstance
{
    public static ConnectionMultiplexer GetMultiplexer() => ConnectionMultiplexer.Connect(ConnectionString);


    public const string ConnectionString = "localhost:6379,allowAdmin=true";
}