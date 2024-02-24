using System.Collections.Concurrent;
using System.Collections.Frozen;

namespace Ephemerally.Redis.Xunit;

public class FixedSizeObjectPool<T>
{
    private readonly FrozenDictionary<T, SemaphoreSlim> _objects;
    private readonly BlockingCollection<T> _availableObjects;

    public IEnumerable<T> Objects => _objects.Keys;

    public FixedSizeObjectPool(ICollection<T> objects)
    {
        _objects = objects.ToFrozenDictionary(x => x, _ => new SemaphoreSlim(1, 1));
        _availableObjects = new BlockingCollection<T>(objects.Count);
        foreach (var obj in objects)
        {
            _availableObjects.Add(obj);
        }
    }

    public T Get()
    {
        var obj = _availableObjects.Take();
        if (!_objects.TryGetValue(obj, out var semaphore))
            throw new InvalidOperationException("Object not from pool");

        semaphore.Wait();
        return obj;
    }

    public void Return(T obj)
    {
        if (!_objects.TryGetValue(obj, out var semaphore))
            throw new InvalidOperationException("Object not from pool");

        try
        {
            semaphore.Release();
        }
        catch (SemaphoreFullException)
        {
            throw new InvalidOperationException("Object already returned");
        }
            

        if (!_availableObjects.TryAdd(obj))
            throw new InvalidOperationException("Object already returned");
    }
}