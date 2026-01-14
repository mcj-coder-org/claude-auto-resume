namespace McjCoderOrg.ClaudeAutoResume.E2ETests.Helpers;

/// <summary>
/// Represents the result of running a process.
/// </summary>
public sealed record ProcessResult(
    int ExitCode,
    string StandardOutput,
    string StandardError)
{
    /// <summary>
    /// Gets the combined standard output and error.
    /// </summary>
    public string CombinedOutput => StandardOutput + StandardError;
}
