namespace Ephemerally.EnvironmentVariables;

public static class PublicExtensions
{
    public static EnvironmentVariableEphemeral EnvironmentVariable(this IEphemeralExtension _, string variable, string value)
    {
        Environment.SetEnvironmentVariable(variable, value);
        return new(variable);
    }

    public static EnvironmentVariableEphemeral EnvironmentVariable(this IEphemeralExtension _, string variable) => new(variable);
}