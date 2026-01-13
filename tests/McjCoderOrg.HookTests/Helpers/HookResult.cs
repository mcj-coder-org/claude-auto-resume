namespace McjCoderOrg.HookTests.Helpers;

/// <summary>
/// Result of running a hook script.
/// </summary>
public sealed record HookResult(int ExitCode, string Stdout, string Stderr)
{
    /// <summary>
    /// Gets the combined output (stdout + stderr).
    /// </summary>
    public string CombinedOutput => $"{Stdout}{Stderr}";

    /// <summary>
    /// Gets a value indicating whether the hook succeeded (exit code 0).
    /// </summary>
    public bool Success => ExitCode == 0;
}
