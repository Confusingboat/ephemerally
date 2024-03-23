using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using Ephemerally.Redis.Xunit;

namespace Ephemerally.Redis.Tests;

public abstract class EphemeralRedisFixture : TestContainerFixture, IEphemeralRedisFixture
{
    public ushort PublicPort => Container.GetMappedPublicPort(6379);

    public string ConnectionString => $"localhost:{PublicPort},allowAdmin=true";
}

// ReSharper disable once InconsistentNaming
public class EphemeralRedisInstance6 : EphemeralRedisFixture
{
    protected override IContainer CreateContainer() =>
        new ContainerBuilder()
            .WithImage("redis:6-alpine")
            .WithPortBinding(6379, true)
            .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(6379))
            .Build();
}

// ReSharper disable once InconsistentNaming
public class EphemeralRedisInstance7 : EphemeralRedisFixture
{
    protected override IContainer CreateContainer() =>
        new ContainerBuilder()
            .WithImage("redis:7-alpine")
            .WithPortBinding(6379, true)
            .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(6379))
            .Build();
}