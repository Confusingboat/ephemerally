using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Ephemerally;

internal static class InternalExtensions
{
    public static ConfiguredTaskAwaitable F(this Task task) => task.ConfigureAwait(false);

    public static ConfiguredTaskAwaitable<T> F<T>(this Task<T> task) => task.ConfigureAwait(false);

    public static ConfiguredValueTaskAwaitable F(this ValueTask task) => task.ConfigureAwait(false);

    public static ConfiguredValueTaskAwaitable<T> F<T>(this ValueTask<T> task) => task.ConfigureAwait(false);
}