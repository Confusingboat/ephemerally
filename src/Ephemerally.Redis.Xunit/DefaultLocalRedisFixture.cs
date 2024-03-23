using Xunit;

namespace Ephemerally.Redis.xUnit;

public interface IRedisInstance
{
    string ConnectionString { get; }
}

public interface IRedisInstanceFixture : IRedisInstance, IAsyncLifetime { }

public sealed class UnmanagedDefaultLocalRedisInstanceFixture : IRedisInstanceFixture
{
    private static readonly Lazy<UnmanagedDefaultLocalRedisInstanceFixture> _instance = new(() => new UnmanagedDefaultLocalRedisInstanceFixture());

    public static UnmanagedDefaultLocalRedisInstanceFixture DefaultLocalRedisInstanceFixture => _instance.Value;

    private UnmanagedDefaultLocalRedisInstanceFixture() { }

    public string ConnectionString => DefaultLocalRedisInstance.ConnectionString;
    public Task InitializeAsync() => Task.CompletedTask;

    public Task DisposeAsync() => Task.CompletedTask;
}

public abstract class RedisInstanceFixture : IRedisInstanceFixture
{
    public Task InitializeAsync() => throw new NotImplementedException();

    public Task DisposeAsync() => throw new NotImplementedException();

    public string ConnectionString { get; }
}