namespace Ephemerally.Redis.Tests;

public class RedisFactAttribute : FactAttribute
{
    public override int Timeout => 2_000;
}