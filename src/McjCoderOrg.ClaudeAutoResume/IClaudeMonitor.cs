namespace McjCoderOrg.ClaudeAutoResume;

/// <summary>
/// Interface for the Claude monitor that wraps Claude Code in a pseudo-terminal.
/// </summary>
internal interface IClaudeMonitor : IDisposable
{
    /// <summary>
    /// Runs the Claude monitor with the specified additional arguments.
    /// </summary>
    /// <param name="additionalArgs">Additional arguments to pass to Claude.</param>
    /// <returns>True if claude was found and executed; false if claude was not found.</returns>
    Task<bool> RunAsync(IReadOnlyList<string> additionalArgs);
}
