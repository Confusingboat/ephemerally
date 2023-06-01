namespace Ephemerally;

public record EphemeralOptions
{
    public CleanupBehavior CleanupBehavior { get; init; } = CleanupBehavior.SelfAndExpired;
}