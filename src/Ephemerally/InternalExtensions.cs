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
}