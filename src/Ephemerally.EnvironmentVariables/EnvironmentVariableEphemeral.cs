namespace Ephemerally.EnvironmentVariables;

public class EnvironmentVariableEphemeral : Ephemeral<string>, IDisposable
{
    public EnvironmentVariableEphemeral(
        string value,
        EphemeralOptions options)
        : base(value, _ => string.Empty, options) { }

    protected override Task CleanupSelfAsync() => throw new NotImplementedException();

    protected override Task CleanupAllAsync()
    {
        CleanupAll();
        return Task.CompletedTask;
    }

    protected override void CleanupSelf() => Cleanup(Value);

    protected override void CleanupAll() { }

    private static void Cleanup(string variable) => Environment.SetEnvironmentVariable(variable, null, EnvironmentVariableTarget.Machine);
}