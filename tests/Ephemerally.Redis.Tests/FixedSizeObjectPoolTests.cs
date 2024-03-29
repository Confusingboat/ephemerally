using System.Diagnostics.CodeAnalysis;
using Shouldly;

namespace Ephemerally.Redis.Tests;

[Collection(FixedSizeObjectPoolTestCollection.Name)]
public class FixedSizeObjectPoolTests
{
    private const int TestTimeout = 5;

    [Fact(Timeout = TestTimeout)]
    public void Get_returns_object_from_pool()
    {
        // Arrange
        int[] items = [1, 2, 3];
        var pool = new FixedSizeObjectPool<int>(items);

        // Act
        var obj = pool.Get();

        // Assert
        items.ShouldContain(obj);
    }

    [Fact(Timeout = TestTimeout)]
    public void Return_throws_if_object_not_from_pool()
    {
        // Arrange
        var pool = new FixedSizeObjectPool<int>([1, 2, 3]);

        // Act & Assert
        Should.Throw<ObjectNotFromPoolException>(() => pool.Return(4));
    }

    [Fact(Timeout = TestTimeout)]
    public void Return_throws_if_object_already_returned()
    {
        // Arrange
        var pool = new FixedSizeObjectPool<int>([1, 2, 3]);
        var obj = pool.Get();
        pool.Return(obj);

        // Act & Assert
        Should.Throw<ObjectAlreadyReturnedException>(() => pool.Return(obj));
    }

    [Fact(Timeout = TestTimeout)]
    public void Get_can_be_called_once_for_each_item_in_pool()
    {
        // Arrange
        var pool = new FixedSizeObjectPool<int>([1, 2, 3]);

        // Act
        var one = pool.Get();
        var two = pool.Get();
        var three = pool.Get();

        // Assert
        one.ShouldBe(1);
        two.ShouldBe(2);
        three.ShouldBe(3);
    }

    [Fact(Timeout = TestTimeout)]
    public void Get_can_be_called_once_for_each_duplicate_instance()
    {
        // Arrange
        var pool = new FixedSizeObjectPool<int>([1, 1, 1]);

        // Act
        var one = pool.Get();
        var two = pool.Get();
        var three = pool.Get();

        // Assert
        one.ShouldBe(1);
        two.ShouldBe(1);
        three.ShouldBe(1);
    }

    [Fact(Timeout = TestTimeout)]
    public void Return_can_be_called_once_for_each_duplicate_instance()
    {
        // Arrange
        var pool = new FixedSizeObjectPool<int>([1, 1, 1]);
        pool.Get();
        pool.Get();
        pool.Get();

        // Act & Assert
        Should.NotThrow(() =>
        {
            pool.Return(1);
            pool.Return(1);
            pool.Return(1);
        });
    }

    [Fact(Timeout = TestTimeout)]
    public void GetWhere_should_return_first_matching_object()
    {
        // Arrange
        var pool = new FixedSizeObjectPool<int>([1, 2, 3]);

        // Act
        var obj = pool.GetWhere(i => i == 2);

        // Assert
        obj.ShouldBe(2);
    }

    [Fact(Timeout = TestTimeout)]
    public void GetWhere_should_throw_when_no_matching_objects_are_in_pool()
    {
        // Arrange
        var pool = new FixedSizeObjectPool<int>([1, 2, 3]);

        // Act & Assert
        Should.Throw<ObjectNotFromPoolException>(() => pool.GetWhere(i => i == 4));
    }

    [Fact(Timeout = TestTimeout)]
    public void GetWhere_should_return_second_matching_object()
    {
        // Arrange
        var pool = new FixedSizeObjectPool<int>([1, 2, 3, 4]);
        var two = pool.GetWhere(i => i == 2);

        // Act
        var obj = pool.GetWhere(i => i % 2 == 0);

        // Assert
        obj.ShouldBe(4);
    }

    [Fact(Timeout = TestTimeout * 10), SuppressMessage("ReSharper", "MethodSupportsCancellation")]
    public async Task GetWhere_should_wait_for_first_available_matching_object()
    {
        // Arrange
        var pool = new FixedSizeObjectPool<int>([1, 2, 3, 4]);
        pool.Get();
        pool.GetWhere(i => i == 2);
        using var cts = new CancellationTokenSource();

        var task1 = Task.Run(async () =>
        {
            try
            {
                await Task.Delay(Timeout.Infinite, cts.Token);
            }
            catch (TaskCanceledException) { /* Expected */ }
            pool.Return(2);
        });
        pool.GetWhere(i => i == 4);

        // Act
        var task2 = Task.Run(() => pool.GetWhere(i => i == 2));
        await Task.Delay(1);
        await cts.CancelAsync();

        // Assert
        var actual = await task2;
        actual.ShouldBe(2);
        await task1;
    }
}

[CollectionDefinition(Name, DisableParallelization = true)]
public class FixedSizeObjectPoolTestCollection
{
    public const string Name = nameof(FixedSizeObjectPoolTestCollection);
}