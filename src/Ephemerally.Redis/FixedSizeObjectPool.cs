using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Frozen;

namespace Ephemerally.Redis;

public class FixedSizeObjectPool<T>
{
    private readonly FrozenDictionary<T, SemaphoreSlim> _objects;

    public IEnumerable<T> Objects => _objects.Keys;

    public FixedSizeObjectPool(ICollection<T> objects)
    {
        _objects = objects
            .GroupBy(x => x)
            .Select(x => (x.Key, Count: x.Count()))
            .ToFrozenDictionary(x => x.Key, x => new SemaphoreSlim(x.Count, x.Count));
    }

    public T Get() => GetWhere(x => true);

    public T GetWhere(Func<T, bool> predicate)
    {
        var candidates = _objects
            .Where(kv => predicate(kv.Key))
            .Select(kv => (Obj: kv.Key, WaitHandle: kv.Value.AvailableWaitHandle))
            .ToList();

        if (candidates.Count == 0)
            throw new ObjectNotFromPoolException();

        var candidateIndex = WaitHandle.WaitAny(
            candidates
                .Select(x => x.WaitHandle)
                .ToArray());

        return GetInternal(candidates[candidateIndex].Obj);
    }

    private T GetInternal(T obj)
    {
        if (!_objects.TryGetValue(obj, out var semaphore))
            throw new ObjectNotFromPoolException();

        semaphore.Wait();

        return obj;
    }

    public void Return(T obj)
    {
        if (!_objects.TryGetValue(obj, out var semaphore))
            throw new ObjectNotFromPoolException();

        try
        {
            semaphore.Release();
        }
        catch (SemaphoreFullException)
        {
            throw new ObjectAlreadyReturnedException();
        }
    }
}

public class ObjectNotFromPoolException() : InvalidOperationException("Object not from pool");

public class ObjectAlreadyReturnedException() : InvalidOperationException("Object already returned");