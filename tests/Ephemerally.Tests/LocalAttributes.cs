using Xunit;
using static Ephemerally.Tests.LocalExtensions;
using TheoryAttribute = Xunit.TheoryAttribute;

// ReSharper disable InconsistentNaming, VirtualMemberCallInConstructor

namespace Ephemerally.Tests;

public class LocalFactAttribute : FactAttribute
{
    public LocalFactAttribute()
    {
        if (IsCI())
        {
            Skip = "Local only";
        }
    }
}

public class LocalTheoryAttribute : TheoryAttribute
{
    public LocalTheoryAttribute()
    {
        if (IsCI())
        {
            Skip = "Local only";
        }
    }
}

file static class LocalExtensions
{
    public static bool IsCI() => Environment.GetEnvironmentVariable("CI") is not null;
}