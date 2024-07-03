namespace Ephemerally.EnvironmentVariables;

public class EnvironmentVariableEphemeral : Ephemeral<string>, IDisposable
{
    public EnvironmentVariableEphemeral(string variable) : this(variable, new EphemeralOptions()) { }

    public EnvironmentVariableEphemeral(
        string variable,
        EphemeralOptions options)
        : base(variable, _ => string.Empty, options) { }

    protected override Task CleanupSelfAsync()
    {
        CleanupSelf();
        return Task.CompletedTask;
    }

    protected override Task CleanupAllAsync()
    {
        CleanupAll();
        return Task.CompletedTask;
    }

    protected override void CleanupSelf() => Cleanup(Value);

    protected override void CleanupAll() { }

    private static void Cleanup(string variable) => Environment.SetEnvironmentVariable(variable, null, EnvironmentVariableTarget.Machine);
}