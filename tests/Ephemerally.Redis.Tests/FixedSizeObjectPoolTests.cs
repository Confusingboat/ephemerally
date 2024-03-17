using Shouldly;

namespace Ephemerally.Redis.Tests;

public class FixedSizeObjectPoolTests
{
    [Fact]
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

    [Fact(Timeout = 2)]
    public void Return_throws_if_object_not_from_pool()
    {
        // Arrange
        var pool = new FixedSizeObjectPool<int>([1, 2, 3]);

        // Act & Assert
        Should.Throw<ObjectNotFromPoolException>(() => pool.Return(4));
    }

    [Fact(Timeout = 2)]
    public void Return_throws_if_object_already_returned()
    {
        // Arrange
        var pool = new FixedSizeObjectPool<int>([1, 2, 3]);
        var obj = pool.Get();
        pool.Return(obj);

        // Act & Assert
        Should.Throw<ObjectAlreadyReturnedException>(() => pool.Return(obj));
    }

    [Fact(Timeout = 2)]
    public void Get_can_be_called_once_for_each_item_in_pool()
    {
        // Arrange
        var pool = new FixedSizeObjectPool<int>([1, 2, 3]);

        // Act
        pool.Get();
        pool.Get();
        pool.Get();

        // If test does not time out, it passes
    }
}