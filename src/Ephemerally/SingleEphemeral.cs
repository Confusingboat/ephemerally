using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ephemerally;

public sealed class SingleEphemeral<T> : Ephemeral<T> where T : class
{
    private readonly Func<Task> _cleanupSelfAsync;

    public SingleEphemeral(
        T value,
        Func<Task> cleanupSelfAsync) 
        : base(value, x => string.Empty, new EphemeralOptions { CleanupBehavior = CleanupBehavior.SelfOnly })
    {
        _cleanupSelfAsync = cleanupSelfAsync;
    }

    protected override Task CleanupSelfAsync() => _cleanupSelfAsync();

    protected override Task CleanupAllAsync() => Task.CompletedTask;
}