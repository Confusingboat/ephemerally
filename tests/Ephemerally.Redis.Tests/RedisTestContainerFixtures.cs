using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using Ephemerally.Redis.Xunit;

namespace Ephemerally.Redis.Tests;

public abstract class RedisTestContainerFixture : TestContainerFixture, IRedisTestContainerFixture
{
    public ushort PublicPort => Container.GetMappedPublicPort(6379);

    public string ConnectionString => $"localhost:{PublicPort},allowAdmin=true";
}

// ReSharper disable once InconsistentNaming
public class RedisTestContainerFixture_6 : RedisTestContainerFixture
{
    protected override IContainer CreateContainer() =>
        new ContainerBuilder()
            .WithImage("redis:6-alpine")
            .WithPortBinding(6379, true)
            .Build();
}

// ReSharper disable once InconsistentNaming
public class RedisTestContainerFixture_7 : RedisTestContainerFixture
{
    protected override IContainer CreateContainer() =>
        new ContainerBuilder()
            .WithImage("redis:7-alpine")
            .WithPortBinding(6379, true)
            .Build();
}