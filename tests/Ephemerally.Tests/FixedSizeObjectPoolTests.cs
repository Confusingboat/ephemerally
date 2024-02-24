using Ephemerally.Redis.Xunit;

namespace Ephemerally.Tests;

public class FixedSizeObjectPoolTests
{
    [Test]
    public void Get_Returns_Object_From_Pool()
    {
        int[] items = [1, 2, 3];
        var pool = new FixedSizeObjectPool<int>(items);
        var obj = pool.Get();
        Assert.That(items, Does.Contain(obj));
    }

    [Test]
    public void Return_Throws_If_Object_Not_From_Pool()
    {
        var pool = new FixedSizeObjectPool<int>([1, 2, 3]);
        Assert.Throws<InvalidOperationException>(() => pool.Return(4));
    }

    [Test]
    public void Return_Throws_If_Object_Already_Returned()
    {
        var pool = new FixedSizeObjectPool<int>([1, 2, 3]);
        var obj = pool.Get();
        pool.Return(obj);
        Assert.Throws<InvalidOperationException>(() => pool.Return(obj));
    }
}