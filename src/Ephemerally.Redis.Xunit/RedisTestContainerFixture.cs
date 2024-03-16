using Xunit;

namespace Ephemerally.Redis.Xunit;

public interface IRedisTestContainerFixture : IAsyncLifetime
{
    ushort PublicPort { get; }
    string ConnectionString { get; }
}

public sealed class UnmanagedTestContainerFixture : IRedisTestContainerFixture
{
    private static readonly Lazy<UnmanagedTestContainerFixture> _instance = new(() => new UnmanagedTestContainerFixture());

    public static UnmanagedTestContainerFixture Instance => _instance.Value;

    private UnmanagedTestContainerFixture() { }

    public ushort PublicPort => DefaultLocalRedisInstance.Port;
    public string ConnectionString => DefaultLocalRedisInstance.ConnectionString;
    public Task InitializeAsync() => Task.CompletedTask;

    public Task DisposeAsync() => Task.CompletedTask;
}