using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ephemerally;

internal static class InternalExtensions
{
    internal static T OrDefault<T>(this T options) where T : EphemeralOptions, new() =>
        options ?? new T();

    public static ValueTask TryDisposeAsync<T>(this T self) where T : class =>
        self is IAsyncDisposable disposable
            ? disposable.DisposeAsync()
            : new();

    public static void TryDispose<T>(this T self) where T : class
    {
        (self as IDisposable)?.Dispose();
    }
}