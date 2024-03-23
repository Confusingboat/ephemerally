using Xunit;

namespace Ephemerally.Redis.Xunit;

public interface IEphemeralRedisFixture : IAsyncLifetime
{
    string ConnectionString { get; }
}

public sealed class UnmanagedFixture : IEphemeralRedisFixture
{
    private static readonly Lazy<UnmanagedFixture> _instance = new(() => new UnmanagedFixture());

    public static UnmanagedFixture Fixture => _instance.Value;

    private UnmanagedFixture() { }

    public string ConnectionString => DefaultLocalRedisInstance.ConnectionString;
    public Task InitializeAsync() => Task.CompletedTask;

    public Task DisposeAsync() => Task.CompletedTask;
}