using StackExchange.Redis;

namespace Ephemerally.Redis;

internal static class InternalExtensions
{
    public static IConnectionMultiplexer GetRootMultiplexer(this IConnectionMultiplexer multiplexer)
    {
        while (true)
        {
            if (multiplexer is not ConnectionMultiplexerDecorator decorator) return multiplexer;

            multiplexer = decorator.UnderlyingMultiplexer;
        }
    }
}