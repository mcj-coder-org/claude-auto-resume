namespace McjCoderOrg.ClaudeAutoResume;

/// <summary>
/// Main application entry point interface.
/// </summary>
internal interface IApplication
{
    /// <summary>
    /// Runs the application with the specified arguments.
    /// </summary>
    /// <param name="args">Command-line arguments.</param>
    /// <returns>Exit code.</returns>
    Task<int> RunAsync(string[] args);
}
