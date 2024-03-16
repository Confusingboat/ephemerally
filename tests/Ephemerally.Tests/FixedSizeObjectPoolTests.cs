using Ephemerally.Redis.Xunit;

namespace Ephemerally.Tests;

public class FixedSizeObjectPoolTests
{
    [Test]
    public void Get_returns_object_from_pool()
    {
        int[] items = [1, 2, 3];
        var pool = new FixedSizeObjectPool<int>(items);
        var obj = pool.Get();
        Assert.That(items, Does.Contain(obj));
    }

    [Test]
    public void Return_throws_if_object_not_from_pool()
    {
        var pool = new FixedSizeObjectPool<int>([1, 2, 3]);
        Assert.Throws<InvalidOperationException>(() => pool.Return(4));
    }

    [Test]
    public void Return_throws_if_object_already_returned()
    {
        var pool = new FixedSizeObjectPool<int>([1, 2, 3]);
        var obj = pool.Get();
        pool.Return(obj);
        Assert.Throws<InvalidOperationException>(() => pool.Return(obj));
    }

    [Test]
    [Timeout(2)]
    public void Get_can_be_called_once_for_each_item_in_pool()
    {
        var pool = new FixedSizeObjectPool<int>([1, 2, 3]);
        pool.Get();
        pool.Get();
        pool.Get();
    }
}